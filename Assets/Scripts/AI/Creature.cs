using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Creature : BaseInput
{
    protected NavMeshAgent nma_self;
    protected Rigidbody rb;
    private bool b_canAttack = false;
    public bool CanAttack { get { return b_canAttack; } }
    // Start is called before the first frame update
    protected void Start()
    {
        nma_self = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Attack() { }
}
