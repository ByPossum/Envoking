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
    private Vector3 v_target;
    private Vector3 v_currentTarget;
    private bool b_undertakingAction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!b_undertakingAction)
        {
            if(!cr_owner.Incapacitated && cr_owner.FollowingPath != null)
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

    private void DrawPath()
    {
        Debug.Log("Path length: " + nmp_pathToFollow.corners.Length);
        for(int i = 0; i < nmp_pathToFollow.corners.Length; i++)
        {

        }
    }
}
