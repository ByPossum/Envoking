using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class AIController : Controller
{
    [SerializeField] private Creature cr_owner;
    [SerializeField] private float f_movementSpeed;
    private Rigidbody rb;
    protected NavMeshPath nmp_pathToFollow;
    private bool b_undertakingAction;
    private bool b_attack;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        b_attack = cr_owner.CanAttack;
        if (!b_undertakingAction)
        {
            if(!cr_owner.Incapacitated && b_attack)
            {
                cr_owner.Attack();
                b_attack = false;
            }
            else if(!cr_owner.Incapacitated && cr_owner.FollowingPath != null && !b_attack)
            {
                if(cr_owner.FollowingPath.corners.Length > 1)
                {
                    nmp_pathToFollow = cr_owner.FollowingPath;
                    rb.AddForce((nmp_pathToFollow.corners[1] - transform.position).normalized, ForceMode.Impulse);
                    transform.rotation = Quaternion.LookRotation((nmp_pathToFollow.corners[1] - transform.position).normalized - Vector3.Scale(transform.position, Vector3.one - Vector3.up));
                    rb.velocity *= f_movementSpeed;
                }
            }
        }
    }
}
