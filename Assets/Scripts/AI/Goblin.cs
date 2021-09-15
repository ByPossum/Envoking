using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin : Creature
{
    [SerializeField] private float f_minRadius;
    [SerializeField] private float f_maxRadius;
    [SerializeField] private float f_shootingTime;
    [SerializeField] private float f_shootingForce;
    [SerializeField] private GameObject go_bullet;
    private bool b_shootCooldown = false;
    private float hp = 5;
    private Vector3 v_shootingPos;
    protected GoblinActions ga_currentAction;
    public GoblinActions CurrentAction { get { return ga_currentAction; } }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
            Destroy(gameObject);
        if (!b_incapacitated)
        {
            if (!b_shootCooldown && ShootAtPlayer())
            {
                b_canAttack = true;
                StartCoroutine(ShootCooldown());
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
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        if(Vector3.Distance(transform.position, playerPos) > f_minRadius)
        {
            v_shootingPos = playerPos;
            return true;
        }
        return false;
    }

    public override void Attack()
    {
        b_canAttack = false;
        PoolManager pm = UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager);
        GameObject bullet = pm.SpawnObject(go_bullet.name, transform.position + transform.forward.normalized, transform.rotation);
        
        Rigidbody bulrb = bullet.GetComponent<Rigidbody>();
        bulrb.AddForce((v_shootingPos - transform.position).normalized * f_shootingForce, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().SetOwner(this);
    }

    public override void TakeDamage(float _damage)
    {
        hp -= _damage;
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForEndOfFrame();
        b_shootCooldown = true;
        yield return new WaitForSeconds(f_shootingTime);
        b_shootCooldown = false;
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