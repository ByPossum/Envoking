using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Creature : BaseInput
{
    protected NavMeshPath nmp_checkingPath;
    protected NavMeshPath nmp_followingPath;
    protected Rigidbody rb;
    protected bool b_canAttack = false;
    public bool CanAttack { get { return b_canAttack; } }
    public NavMeshPath FollowingPath { get { return nmp_followingPath; } }
    protected bool b_incapacitated = false;
    public bool Incapacitated { get { return b_incapacitated; } }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Attack() { }
    public virtual void Knockback(Vector3 _direction, float _force)
    {
        rb.AddForce(_direction * _force, ForceMode.Impulse);
    }
    public virtual void TakeDamage(float f_damage) { }
}
