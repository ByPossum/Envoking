using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;


public class Dog : Creature, IPickupable
{
    private const float f_heartTime = 1.5f;

    private bool b_inPetPosition = false;
    private bool b_petInterrupt = false;
    [SerializeField] private float f_attackCoolDown;
    [SerializeField] private float f_hitBoxTimer;
    [SerializeField] private float f_attackForce;
    [SerializeField] private float f_attackDamage;
    [SerializeField] private float f_playerDistance;
    [SerializeField] private float f_petTransitionSpeed;
    [SerializeField] private Collider col_hitBox;
    [SerializeField] private LayerMask lm_throwChecker;
    [SerializeField] private GameObject go_loveHearts;
    private DogAnimator da_anim;
    private Creature cr_enemy;
    private DogActions da_currentAction = DogActions.none;
    public DogActions CurrentAction { get { return da_currentAction; } }
    public bool InPetPosition { get { return b_inPetPosition; } }
    public float AttackForce { get { return f_attackForce; } }
    public float AttackDamage { get { return f_attackDamage; } }

    protected override void Start()
    {
        base.Start();
        da_anim = GetComponent<DogAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!b_incapacitated && !CheckExclusiveActions())
        {
            CheckActionToPerform();

        }
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
        if (collision.gameObject.GetComponent<IDogable>() != null)
            collision.gameObject.GetComponent<IDogable>().Interact();
    }

    public override void Attack()
    {
        if(cr_enemy != null)
            rb.AddForce((cr_enemy.transform.position - transform.position).normalized * 20, ForceMode.Impulse);
        StartCoroutine(AttackHitbox());
    }

    private bool CheckActionToPerform()
    {
        if (AbleToAttack())
        {
            da_currentAction = DogActions.attackEnemy;
            da_anim.SetAnimTrigger("Attack");
            return true;
        }
        else if (FindEnemy())
        {
            da_currentAction = DogActions.findEnemy;
            da_anim.SetAnimFloat("Walk", 1f);
            return true;
        }
        else if (FindInteractable())
        {
            da_currentAction = DogActions.findInteractable;
            da_anim.SetAnimFloat("Walk", 1f);
            return true;
        }
        else if (NextToPlayer())
        {
            da_currentAction = DogActions.none;
            da_anim.SetAnimFloat("Walk", 0.0f);
        }
        else if (FindPlayer() && !(Vector3.Distance(transform.position, FindObjectOfType<PlayerController>().transform.position) < f_playerDistance))
        {
            da_currentAction = DogActions.follow;
            da_anim.SetAnimFloat("Walk", 1f);
            return true;
        }
        else
        {
            da_currentAction = DogActions.none;
            da_anim.SetAnimFloat("Walk", 0f);
        }
        return false;
    }

    private bool CheckExclusiveActions()
    {
        switch (da_currentAction)
        {
            case DogActions.attackEnemy:
                return true;
            case DogActions.pickedUp:
                return true;
            case DogActions.stay:
                return true;
            case DogActions.Pet:
                return true;
            default:
                return false;
        }
    }

    private bool NextToPlayer()
    {
        if (Vector3.Distance(transform.position, FindObjectOfType<PlayerController>().transform.position) < f_playerDistance)
        {
            da_currentAction = DogActions.none;
            return true;
        }
        return false;
    }

    private bool FindPlayer()
    {
        nmp_checkingPath = new NavMeshPath();
        Vector3 randVector = Utils.RandomVector3(f_playerDistance, 0f, f_playerDistance, true);
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        randVector = (transform.position - playerPos).normalized * Mathf.Abs(randVector.x) + ((Vector3.Cross(Vector3.up, transform.position - playerPos).normalized * randVector.z));
        if (NavMesh.CalculatePath(transform.position, playerPos + randVector, -1, nmp_checkingPath))
        {
            if(nmp_checkingPath.status == NavMeshPathStatus.PathComplete)
            {
                playerPos = playerPos - transform.position;
                playerPos.y = 0f;
                transform.rotation = Quaternion.LookRotation(playerPos);
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
        // Find closest goblin.
        foreach(Vector3 goblinPos in allGoblinPos)
        {
            if (Vector3.Distance(transform.position, goblinPos) < smallestValueIndex.dist)
                smallestValueIndex = (Vector3.Distance(transform.position, goblinPos), iter);
            iter++;
        }
        if(nmp_checkingPath.status == NavMeshPathStatus.PathComplete)
        {
            cr_enemy = goblins[smallestValueIndex.index];
            Vector3 unitVec = Utils.RandomVector3(1f, 0, 1f, true);
            Vector3 enemyPos = cr_enemy.transform.position;
            unitVec = (enemyPos - transform.position).normalized * Mathf.Abs(unitVec.x) + (Vector3.Cross(Vector3.up, enemyPos - transform.position).normalized * unitVec.z) * 0.7f;
            NavMesh.CalculatePath(transform.position, (unitVec.normalized * Random.Range(1f, f_playerDistance)) + enemyPos, 1, nmp_followingPath);
            nmp_followingPath = nmp_checkingPath;
            enemyPos = enemyPos - transform.position;
            enemyPos.y = 0f;
            transform.rotation = Quaternion.LookRotation(enemyPos);
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
        IDogable[] interactables = FindObjectsOfType<MonoBehaviour>().OfType<IDogable>().ToArray();
        if (interactables.Length == 0)
            return false;
        List<IDogable> pathedInteractables = new List<IDogable>();
        nmp_checkingPath = new NavMeshPath();
        for (int i = 0; i < interactables.Length; i++)
        {
            GameObject obj = interactables[i].GetGameObject();
            if(NavMesh.CalculatePath(transform.position, obj.transform.position, -1, nmp_checkingPath))
            {
                if (nmp_checkingPath.status == NavMeshPathStatus.PathComplete)
                    pathedInteractables.Add(interactables[i]);
            }
        }
        if (pathedInteractables.Count == 0)
            return false;
        GameObject objectToMoveTowards = null;
        float lowestDistance = 100;
        foreach(IDogable inter in pathedInteractables)
            if(Vector3.Distance(transform.position, inter.GetGameObject().transform.position) < lowestDistance)
            {
                objectToMoveTowards = inter.GetGameObject();
                lowestDistance = Vector3.Distance(transform.position, inter.GetGameObject().transform.position);
            }
        if (objectToMoveTowards == null)
            return false;
        nmp_followingPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, objectToMoveTowards.transform.position, -1, nmp_followingPath);
        return true;
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
    public void Pet(Vector3 _posToMoveTo, Vector3 _rot)
    {
        b_incapacitated = true;
        da_currentAction = DogActions.Pet;
        StartCoroutine(GetPet(_posToMoveTo, _rot));
    }

    public void SetInPetPosition()
    {
        b_inPetPosition = !b_inPetPosition;
    }

    public void StopPet()
    {
        b_inPetPosition = false;
        b_incapacitated = false;
        da_currentAction = DogActions.none;
    }

    public void InterruptPetting()
    {
        b_petInterrupt = true;
    }

    public void TurnLoveHeartsOn()
    {
        go_loveHearts.SetActive(true);
        StartCoroutine(TurnOffHearts());
    }

    public override void TakeDamage(float _damage)
    {
        da_currentAction = DogActions.damaged;
    }

    public IEnumerator GetPet(Vector3 _posToMoveTo, Vector3 _rot)
    {
        Vector3 startPoint = transform.position;
        float startTime = Time.time;
        _posToMoveTo = new Vector3(_posToMoveTo.x, transform.position.y, _posToMoveTo.z);
        float journey = Vector3.Distance(startPoint, _posToMoveTo);
        transform.rotation = Quaternion.LookRotation(_rot);
        bool interupt = b_petInterrupt || da_currentAction == DogActions.damaged;
        while(transform.position != _posToMoveTo || interupt)
        {
            float distance = (Time.time - startTime) * f_petTransitionSpeed;
            float t = distance / journey;
            transform.position = Vector3.Lerp(startPoint, _posToMoveTo, t);
            yield return new WaitForEndOfFrame();
        }
        if (!interupt)
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            da_anim.SetAnimTrigger("Pet");
        }
        b_petInterrupt = false;
    }

    public IEnumerator TurnOffHearts()
    {
        yield return new WaitForSeconds(f_heartTime);
        go_loveHearts.SetActive(false);
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
    damaged,
    stay,
    Pet
}
