using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PlayerAnimator))]
public class PlayerController : Controller
{
    private Rigidbody rb;
    private Collider col;
    private PlayerAnimator pa_anim;
    private bool b_canJump = true;
    private bool b_canPickup = true;
    private bool b_canFire = true;
    private GameObject go_heldObject;
    private PlayerAction pa_currentAction = PlayerAction.none;

    [SerializeField] private float f_movementSpeed;
    [SerializeField] private float f_rotateSpeed;
    [SerializeField] private float f_shotSpeed;
    [SerializeField] private float f_throwForce;
    [SerializeField] private float f_jumpForce;
    [SerializeField] private Transform v_pickupTransform;
    [SerializeField] private GameObject go_bullet;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask lm_pickupLayers;
    [SerializeField] private LayerMask lm_groundChecker;

    public PlayerAction CurrentAction { get { return pa_currentAction; } }
    public bool SetCanJump { set { b_canJump = value; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        bi_input = GetComponent<PlayerInput>();
        pa_anim = GetComponent<PlayerAnimator>();
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
            pa_anim.SetAnimTrigger("Attack");
        // Pickup Check
        if (bi_input.Actions.y > 0 && b_canPickup)
            Pickup();
        // Petting Dog Check (This code needs to run on Group A and not on Group B)
        if (bi_input.Special > 0 && CheckExclusiveActions())
            StrokeDog();
    }

    private void FixedUpdate()
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

    private void FireBullet()
    {
        PoolManager pm = UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager);
        GameObject bull = pm.SpawnObject(go_bullet.name, transform.position + (transform.forward), Quaternion.identity);
        bull.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
        bull.GetComponent<Bullet>().SetOwner(bi_input);
        b_canFire = false;
        StartCoroutine(FireCooldown());
    }

    private void Pickup()
    {
        switch (pa_currentAction)
        {
            case PlayerAction.holding:
                if(go_heldObject != null)
                {
                    pa_anim.SetAnimTrigger("Throw");
                    pa_anim.SetAnimBool("Carry", false);
                    go_heldObject.transform.parent = null;
                    go_heldObject.GetComponent<Collider>().isTrigger = false;
                    Rigidbody heldObjRb = go_heldObject.GetComponent<Rigidbody>();
                    heldObjRb.isKinematic = false;
                    heldObjRb.AddForce(transform.forward * f_throwForce, ForceMode.Impulse);
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
                        pa_anim.SetAnimBool("Carry", true);
                        pa_anim.SetAnimLayerWeight(1, 1);
                        hit.transform.GetComponent<IPickupable>().Pickup();
                        hit.transform.position = v_pickupTransform.position;
                        go_heldObject = hit.transform.gameObject;
                        go_heldObject.GetComponent<Rigidbody>().isKinematic = true;
                        go_heldObject.GetComponent<Collider>().isTrigger = true;
                        go_heldObject.transform.parent = transform;
                    }
                }
                pa_currentAction = PlayerAction.holding;
                break;
        }
        b_canPickup = false;
        StartCoroutine(PickupCooldown());
    }

    private void StrokeDog()
    {

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

    public void TakeDamage(float _damage)
    {
        pa_anim.SetAnimTrigger("Damage");
    }

    public void ResetJump()
    {
        b_canJump = true;
    }
    public void Jump()
    {
        rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
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
    stroking
}