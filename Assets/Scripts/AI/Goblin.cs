using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin : Creature
{
    [SerializeField] private float f_maxHealth;
    [SerializeField] private float f_minRadius;
    [SerializeField] private float f_maxRadius;
    [SerializeField] private float f_shootingTime;
    [SerializeField] private float f_shootingForce;
    [SerializeField] private GameObject go_bullet;
    private GoblinAnimator ga_anim;
    private bool b_shootCooldown = false;
    private float hp;
    private Vector3 v_shootingPos;
    protected GoblinActions ga_currentAction;
    public GoblinActions CurrentAction { get { return ga_currentAction; } }

    protected override void Start()
    {
        base.Start();
        ga_anim = GetComponent<GoblinAnimator>();
    }

    private void Awake()
    {
        hp = f_maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            ga_anim.SetAnimTrigger("Die");
        }
        if (!b_incapacitated)
        {
            if (ShootAtPlayer())
            {
                ga_anim.SetAnimTrigger("Attack");
            }
            else if (CheckIfBeingAttacked())
            {

            }
            else if (FindPointDistanceAwayFromPlayer())
            {
                nmp_followingPath = nmp_checkingPath;
            }
        }
        if (transform.position.y < -10f)
            Death();
    }

    private bool FindPointDistanceAwayFromPlayer()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        if (Vector3.Distance(transform.position, playerPos) < f_minRadius)
        {
            Vector3 unitVec = Utils.RandomVector3(1f, 0.0f, 1f, true);
            unitVec = (transform.position - playerPos).normalized * Mathf.Abs(unitVec.x) + (Vector3.Cross(Vector3.up, transform.position - playerPos).normalized * unitVec.z) * 0.7f;
            nmp_checkingPath = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, (unitVec.normalized * Random.Range(f_minRadius, f_maxRadius)) + playerPos, 1, nmp_checkingPath))
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
        PlayerController player = FindObjectOfType<PlayerController>();
        Vector3 playerPos = player.transform.position;
        if (player.CheckUninteruptableActions() && Vector3.Distance(transform.position, playerPos) > f_minRadius)
        {
            v_shootingPos = playerPos;
            Vector3 rot = playerPos - transform.position;
            rot.y = 0f;
            transform.rotation = Quaternion.LookRotation(rot);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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

    public void Death()
    {
        UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).ReturnToPool(gameObject);
        rb.velocity = Vector3.zero;
        hp = f_maxHealth;
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