using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin : Creature
{
    [SerializeField] private float f_minRadius;
    [SerializeField] private float f_maxRadius;
    private float hp = 5;
    protected GoblinActions ga_currentAction;
    public GoblinActions CurrentAction { get { return ga_currentAction; } }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
            Destroy(gameObject);
        if (!b_incapacitated)
        {
            if (ShootAtPlayer())
            {

            }
            else if (CheckIfBeingAttacked())
            {

            }
            else if (FindPointDistanceAwayFromPlayer())
            {
                nmp_followingPath = nmp_checkingPath;
            }
        }
    }

    private bool FindPointDistanceAwayFromPlayer()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        if (Vector3.Distance(transform.position, playerPos) < f_minRadius)
        {
            float randomUnit = Random.Range(-1, 1);
            Vector3 unitVec = new Vector3(randomUnit, 0.0f, randomUnit);
            nmp_checkingPath = new NavMeshPath();
            if(NavMesh.CalculatePath(transform.position, (unitVec.normalized * Random.Range(f_minRadius, f_maxRadius)) + playerPos, 1, nmp_checkingPath))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfBeingAttacked()
    {
        return false;
    }

    private bool ShootAtPlayer()
    {
        return false;
    }

    public override void Attack()
    {
        b_canAttack = false;
    }

    public void TakeDamage(float _damage)
    {
        hp -= _damage;
    }
}

public enum GoblinActions
{
    none,
    idle,
    avoidPlayer,
    avoidDog,
    attacking,
}