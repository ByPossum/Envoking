using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : Controller
{
    private Rigidbody rb;
    private Collider col;
    private bool b_canPickup = true;
    private bool b_canFire = true;
    private GameObject go_heldObject;
    private PlayerAction pa_currentAction = PlayerAction.none;

    [SerializeField] private float f_movementSpeed;
    [SerializeField] private float f_rotateSpeed;
    [SerializeField] private float f_shotSpeed;
    [SerializeField] private float f_throwForce;
    [SerializeField] private Transform v_pickupTransform;
    [SerializeField] private GameObject go_bullet;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask lm_pickupLayers;

    public PlayerAction CurrentAction { get { return pa_currentAction; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        bi_input = GetComponent<BaseInput>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IUsable>() != null)
            collision.gameObject.GetComponent<IUsable>().Interact();
    }

    // Update is called once per frame
    void Update()
    {
        if (bi_input.Actions.x > 0 && CheckExclusiveActions() && b_canFire)
            FireBullet();
        if (bi_input.Actions.y > 0 && b_canPickup)
            Pickup();
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

        // Movement
        rb.AddForce((bi_input.Movement.normalized * Time.deltaTime) * f_movementSpeed, ForceMode.Impulse);
        rb.velocity = (bi_input.Movement != Vector3.zero ? Vector3.ClampMagnitude(rb.velocity, 10f) : Vector3.zero + Physics.gravity);

        // Action Checking
        if (rb.velocity.x != 0.0f || rb.velocity.z != 0.0f && CheckExclusiveActions())
            pa_currentAction = PlayerAction.walking;
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
                if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, 1f, lm_pickupLayers))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.GetComponent<IPickupable>() != null)
                    {
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

    }
}

public enum PlayerAction
{
    none,
    idle,
    walking,
    shooting,
    holding,
    stroking
}