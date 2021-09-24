using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PlayerAnimator))]
public class PlayerController : Controller
{
    private const int i_MAXLIVES = 3;

    private float f_health;
    private bool b_canJump = true;
    private bool b_canPickup = true;
    private bool b_canFire = true;
    private bool b_shouldGroundCheck = false;
    private int i_currentLives;
    private Vector3 v_currentSpawn;
    private Collider col;
    private Rigidbody rb;
    private GameObject go_heldObject;
    private PlayerAnimator pa_anim;
    private SkinnedMeshRenderer sm;
    private PlayerAction pa_currentAction = PlayerAction.none;

    [SerializeField] private Vector3 v_defaultRespawn;
    [SerializeField] private float f_maxHealth;
    [SerializeField] private float f_movementSpeed;
    [SerializeField] private float f_rotateSpeed;
    [SerializeField] private float f_shotSpeed;
    [SerializeField] private float f_throwForce;
    [SerializeField] private float f_jumpForce;
    [SerializeField] private Transform v_pickupTransform;
    [SerializeField] private GameObject go_bullet;
    [SerializeField] private GameObject go_damageParticles;
    [SerializeField] private Transform t_petPoint;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask lm_pickupLayers;
    [SerializeField] private LayerMask lm_groundChecker;
    [SerializeField] private LayerMask lm_ignoreLayer;

    public float CurrentHealth { get { return f_health; } }
    public float MaxHealth { get { return f_maxHealth; } }
    public PlayerAction CurrentAction { get { return pa_currentAction; } }
    public bool SetCanJump { set { b_canJump = value; } }
    public Vector3 SpawnPoint { set { v_currentSpawn = value; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        bi_input = GetComponent<PlayerInput>();
        pa_anim = GetComponent<PlayerAnimator>();
        sm = GetComponentInChildren<SkinnedMeshRenderer>();
        v_currentSpawn = v_defaultRespawn;
        i_currentLives = i_MAXLIVES;
        f_health = f_maxHealth;
        Physics.IgnoreLayerCollision(gameObject.layer, (int)Mathf.Log(lm_ignoreLayer.value,2f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IUsable>() != null)
            collision.gameObject.GetComponent<IUsable>().Interact();
    }

    // Update is called once per frame
    void Update()
    {
        // Attack Check
        if (bi_input.Actions.x > 0 && CheckExclusiveActions() && b_canFire)
        {
            pa_anim.SetAnimLayerWeight(1, 1f);
            pa_anim.SetAnimTrigger("Attack");
        }
        // Pickup Check
        if (bi_input.Actions.y > 0 && b_canPickup)
            Pickup();
        // Petting Dog Check (This code needs to run on Group A and not on Group B)
        if (bi_input.Special && CheckExclusiveActions())
            StrokeDog();
        if (transform.position.y < -10f)
            transform.position = v_currentSpawn;
    }

    private void FixedUpdate()
    {
        if(pa_currentAction != PlayerAction.stroking)
        {
            // Mouse based rotation
            RaycastHit hit;
            Physics.Raycast(cam.ScreenPointToRay(bi_input.Look), out hit);
            Vector3 lookPoint = hit.point;
            lookPoint.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(lookPoint - Vector3.Scale(transform.position, Vector3.one - Vector3.up));
            // Jump Check (Happens in FixedUpdate due to applying force within an animation event)
            if (bi_input.Jump && b_canJump && CheckExclusiveActions())
            {
                pa_anim.SetAnimTrigger("Jump");
                b_canJump = false;
                pa_currentAction = PlayerAction.jumping;
                StartCoroutine(GroundCheckTime());
            }
            else if (!b_canJump)
            {
                // Check if there is an object beneath the player
                RaycastHit groundCheck;
                if(Physics.Raycast(transform.position, Vector3.down, out groundCheck, 0.2f))
                {
                    if (!groundCheck.collider.GetComponent<PlayerController>())
                    {
                        b_canJump = true;
                        pa_anim.SetAnimTrigger("Fall");
                        b_shouldGroundCheck = false;
                    }
                }
            }
            // Movement
            rb.AddForce((bi_input.Movement.normalized * Time.deltaTime) * f_movementSpeed, ForceMode.Impulse);
            rb.velocity = (bi_input.Movement != Vector3.zero || pa_currentAction == PlayerAction.jumping ? Vector3.ClampMagnitude(rb.velocity, 10f) : Vector3.zero + Physics.gravity);
            // Action Checking
            if (bi_input.Movement.x != 0.0f || bi_input.Movement.z != 0.0f && CheckExclusiveActions())
            {
                pa_currentAction = PlayerAction.walking;
                pa_anim.SetAnimFloat("WalkSpeed", Mathf.Abs(bi_input.Movement.x) >= 1 ? Mathf.Abs(bi_input.Movement.x) : Mathf.Abs(bi_input.Movement.z) >= 1 ? Mathf.Abs(bi_input.Movement.z) : 0.0f);
            }
            else if(CheckExclusiveActions())
                pa_currentAction = PlayerAction.idle;
        }
    }

    /// <summary>
    /// Called from animation. Spawns a bullet and fires it forwards
    /// </summary>
    private void Attack()
    {
        // Get bullet from the pool
        PoolManager pm = UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager);
        GameObject bull = pm.SpawnObject(go_bullet.name, transform.position + (transform.forward) * 1.5f, Quaternion.identity);
        // Apply force to bullet
        bull.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
        bull.GetComponent<Bullet>().SetOwner(bi_input);
        // Manage cooldown
        b_canFire = false;
        StartCoroutine(FireCooldown());
    }

    /// <summary>
    /// Lets player pick up and throw objects
    /// </summary>
    private void Pickup()
    {
        switch (pa_currentAction)
        {
            case PlayerAction.holding:
                // If they player isn't holding an object, just reset to idle
                if(go_heldObject != null)
                {
                    // play throwing animation
                    pa_anim.SetAnimTrigger("Throw");
                    pa_anim.SetAnimBool("Carry", false);
                    // Unparent the player from the object and turn its collider back on
                    go_heldObject.transform.parent = null;
                    go_heldObject.GetComponent<Collider>().isTrigger = false;
                    // Apply force to the object
                    Rigidbody heldObjRb = go_heldObject.GetComponent<Rigidbody>();
                    heldObjRb.isKinematic = false;
                    heldObjRb.AddForce(transform.forward * f_throwForce, ForceMode.Impulse);
                    // Let to object be "thrown" and unreference the object
                    go_heldObject.GetComponent<IPickupable>().Throw();
                    go_heldObject = null;
                }
                pa_currentAction = PlayerAction.idle;
                break;
            default:
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, 2f, lm_pickupLayers))
                {
                    if (hit.transform.GetComponent<IPickupable>() != null)
                    {
                        // Play carry animation
                        pa_anim.SetAnimBool("Carry", true);
                        pa_anim.SetAnimLayerWeight(1, 1);
                        // Run pickup commands on the object
                        hit.transform.GetComponent<IPickupable>().Pickup();
                        hit.transform.position = v_pickupTransform.position;
                        // Set the object as a child of the player
                        go_heldObject = hit.transform.gameObject;
                        go_heldObject.GetComponent<Rigidbody>().isKinematic = true;
                        go_heldObject.GetComponent<Collider>().isTrigger = true;
                        go_heldObject.transform.parent = transform;
                    }
                }
                pa_currentAction = PlayerAction.holding;
                break;
        }
        // The player cannot run this function again for a little bit
        b_canPickup = false;
        StartCoroutine(PickupCooldown());
    }

    private void StrokeDog()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 1f, transform.forward, out hit, 2f, lm_pickupLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponent<Dog>() != null)
            {
                pa_currentAction = PlayerAction.stroking;
                pa_anim.ResetAnimValues();
                StartCoroutine(PetDog(hit.collider.GetComponent<Dog>()));
            }
        }
    }

    private bool CheckExclusiveActions()
    {
        switch (pa_currentAction)
        {
            case PlayerAction.holding:
                return false;
            case PlayerAction.stroking:
                return false;
            case PlayerAction.jumping:
                return false;
            case PlayerAction.dead:
                return false;
            case PlayerAction.gameOver:
                return false;
            default:
                return true;
        }
    }

    private IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(f_shotSpeed);
        b_canFire = true;
    }
    private IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(f_shotSpeed);
        b_canPickup = true;
    }

    public bool CheckUninteruptableActions()
    {
        switch (pa_currentAction)
        {
            case PlayerAction.stroking:
                return false;
            case PlayerAction.dead:
                return false;
            case PlayerAction.gameOver:
                return false;
            default:
                return true;
        }
    }

    public void TakeDamage(float _damage)
    {
        f_health -= _damage;
        go_damageParticles.SetActive(true);
        StopCoroutine(StopDamageParticles());
        StartCoroutine(StopDamageParticles());
        if(f_health <= 0f && pa_currentAction != PlayerAction.dead)
        {
            pa_anim.SetAnimLayerWeight(1, 0f);
            pa_anim.SetAnimTrigger("Death");
            pa_currentAction = PlayerAction.dead;
        }
        else
        {
            pa_anim.SetAnimLayerWeight(1, 0f);
            pa_anim.SetAnimTrigger("Damage");
        }
    }

    public void ResetJump()
    {
        b_canJump = true;
    }
    public void Jump()
    {
        rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
    }

    public void Death()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if(i_currentLives > 0)
        {
            transform.position = v_currentSpawn;
            pa_currentAction = PlayerAction.idle;
            i_currentLives--;
            f_health = f_maxHealth;
        }
        else
        {
            pa_currentAction = PlayerAction.gameOver;
            transform.position = Vector3.one * 10f;
            rb.useGravity = false;
            sm.enabled = false;
            this.enabled = false;
        }
    }

    public void ReturnToIdle()
    {
        pa_currentAction = PlayerAction.idle;
    }

    private IEnumerator GroundCheckTime()
    {
        yield return new WaitForSeconds(0.3f);
        b_shouldGroundCheck = true;
    }

    public IEnumerator PetDog(Dog _dogToPet)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        _dogToPet.Pet(t_petPoint.position, -transform.forward);
        bool interrupted = _dogToPet.CurrentAction == DogActions.damaged || pa_currentAction == PlayerAction.damaged;
        while (!_dogToPet.InPetPosition || interrupted)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!interrupted)
        {
            pa_anim.SetAnimTrigger("Pet");
        }
        else
        {
            _dogToPet.InterruptPetting();
        }
    }
    private IEnumerator StopDamageParticles()
    {
        yield return new WaitForSeconds(1f);
        go_damageParticles.SetActive(false);
    }
}

public enum PlayerAction
{
    none,
    idle,
    walking,
    jumping,
    shooting,
    holding,
    stroking,
    damaged,
    dead,
    gameOver
}