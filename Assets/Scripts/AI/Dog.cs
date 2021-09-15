using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Dog : Creature, IPickupable
{
    [SerializeField] private float f_attackCoolDown;
    [SerializeField] private float f_hitBoxTimer;
    [SerializeField] private float f_attackForce;
    [SerializeField] private float f_attackDamage;
    [SerializeField] private LayerMask lm_throwChecker;
    [SerializeField] private Collider col_hitBox;
    private Creature cr_enemy;
    private DogActions da_currentAction = DogActions.none;
    public DogActions CurrentAction { get { return da_currentAction; } }
    public float AttackForce { get { return f_attackForce; } }
    public float AttackDamage { get { return f_attackDamage; } }

    // Update is called once per frame
    void Update()
    {
        if(!b_incapacitated)
            CheckActionToPerform();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (b_incapacitated)
        {
            if(collision.transform.gameObject.layer == Mathf.Log(lm_throwChecker.value,2))
            {
                Debug.Log(collision.transform.name);
                b_incapacitated = false;
            }
        }
    }

    public override void Attack()
    {
        rb.AddForce((cr_enemy.transform.position - transform.position).normalized * 3, ForceMode.Impulse);
        StartCoroutine(AttackHitbox());
    }

    private bool CheckActionToPerform()
    {
        if (AbleToAttack())
        {
            da_currentAction = DogActions.attackEnemy;
            return true;
        }
        else if (FindEnemy())
        {
            da_currentAction = DogActions.findEnemy;
            return true;
        }
        else if (FindInteractable())
        {
            da_currentAction = DogActions.findInteractable;
            return true;
        }
        else if (FindPlayer())
        {
            da_currentAction = DogActions.follow;
            return true;
        }
        da_currentAction = DogActions.stay;
        return false;
    }

    private bool FindPlayer()
    {
        // TODO: Add a distance to make it feel more natural
        nmp_checkingPath = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, FindObjectOfType<PlayerController>().transform.position, -1, nmp_checkingPath))
        {
            if(nmp_checkingPath.status == NavMeshPathStatus.PathComplete)
            {
                nmp_followingPath = nmp_checkingPath;
                return true;
            }
        }
        return false;
    }

    private bool FindEnemy()
    {
        List<Vector3> allGoblinPos = new List<Vector3>();
        (float dist, int index) smallestValueIndex = (100f, 0);
        int iter = 0;
        nmp_checkingPath = new NavMeshPath();
        Goblin[] goblins = FindObjectsOfType<Goblin>();
        // Collect all active goblin positions
        foreach (Goblin gob in goblins)
        {
            if (!gob.enabled)
                continue;
            if (NavMesh.CalculatePath(transform.position, gob.transform.position, -1, nmp_checkingPath))
            {
                if(nmp_checkingPath.status == NavMeshPathStatus.PathComplete)
                {
                    allGoblinPos.Add(gob.transform.position);
                }
            }
        }
        // If there are no available goblins, action cannot be completed
        if(allGoblinPos.Count < 1)
            return false;
        // Find closest goblin. TODO: Add max distance from player or something.
        foreach(Vector3 goblinPos in allGoblinPos)
        {
            if (Vector3.Distance(transform.position, goblinPos) < smallestValueIndex.dist)
                smallestValueIndex = (Vector3.Distance(transform.position, goblinPos), iter);
            iter++;
        }
        if(nmp_checkingPath.status == NavMeshPathStatus.PathComplete)
        {
            cr_enemy = goblins[smallestValueIndex.index];
            nmp_followingPath = nmp_checkingPath;
            return true;
        }
        return false;
    }

    private bool AbleToAttack()
    {
        if(cr_enemy != null)
        {
            if (Vector3.Distance(cr_enemy.transform.position, transform.position) < 2f)
            {
                b_canAttack = true;
                return true;
            }
        }
        return false;
    }

    private bool FindInteractable()
    {
        return false;
    }

    public IEnumerator AttackHitbox()
    {
        col_hitBox.enabled = true;
        yield return new WaitForSeconds(f_hitBoxTimer);
        col_hitBox.enabled = false;
        yield return new WaitForSeconds(f_attackCoolDown);
        cr_enemy = null;
        b_canAttack = false;
    }

    public void Pickup()
    {
        da_currentAction = DogActions.pickedUp;
        b_incapacitated = true;
    }

    public void Throw()
    {
        da_currentAction = DogActions.none;
    }
}

public enum DogActions
{
    none,
    follow,
    findEnemy,
    attackEnemy,
    findInteractable,
    interact,
    pickedUp,
    stay
}
