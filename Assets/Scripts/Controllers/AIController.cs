using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIController : Controller
{
    [SerializeField] private Creature cr_owner;
    [SerializeField] private float f_movementSpeed;
    private Rigidbody rb;
    private Vector3 v_target;
    private Vector3 v_currentTarget;
    private bool b_undertakingAction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        v_target = cr_owner.Movement;
        if (!b_undertakingAction)
        {
            v_currentTarget = v_target;
            b_undertakingAction = true;
            StartCoroutine(MoveTowardsGoal());
        }
    }

    private IEnumerator MoveTowardsGoal()
    {
        while (transform.position != v_currentTarget)
        {
            rb.AddForce((transform.position - v_currentTarget).normalized * f_movementSpeed, ForceMode.Impulse);
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector3.zero;
        b_undertakingAction = false;
    }
}
