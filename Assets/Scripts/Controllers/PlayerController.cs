using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : Controller
{
    private Rigidbody rb;
    private Collider col;
    private bool b_canFire = true;
    [SerializeField] private float f_movementSpeed;
    [SerializeField] private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        bi_input = GetComponent<BaseInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (b_canFire && bi_input.Actions.x > 0)
            FireBullet();
        if (bi_input.Actions.y > 0)
            Pickup();
    }

    private void LateUpdate()
    {
        //transform.LookAt(cam.transform.InverseTransformPoint(bi_input.Look));
        rb.AddForce((bi_input.Movement.normalized * Time.deltaTime) * f_movementSpeed, ForceMode.Impulse);
        rb.velocity = (bi_input.Movement != Vector3.zero ? Vector3.ClampMagnitude(rb.velocity, 10f) : Vector3.zero + Physics.gravity);
    }

    private void FireBullet()
    {
        Debug.Log("I'm firing!");
    }
    private void Pickup()
    {
        Debug.Log("Picking Up");
    }
}
