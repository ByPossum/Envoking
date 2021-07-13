using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Dog : Creature, IPickupable
{
    private DogActions da_currentAction = DogActions.none;
    public DogActions CurrentAction { get { return da_currentAction; } }
    [SerializeField] private LayerMask lm_throwChecker;
    // Update is called once per frame
    void Update()
    {
        if(!b_incapacitated)
            CheckActionToPerform();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if (b_incapacitated)
        {
            if(collision.transform.gameObject.layer == lm_throwChecker.value)
            {
                b_incapacitated = false;
            }
        }
    }

    public override void Attack()
    {
        
    }

    private bool CheckActionToPerform()
    {
        if (FindEnemy())
        {
            da_currentAction = DogActions.findEnemy;
            return true;
        }
        if (AbleToAttack())
        {
            da_currentAction = DogActions.attackEnemy;
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
            nmp_followingPath = nmp_checkingPath;
            return true;
        }
        return false;
    }

    private bool FindEnemy()
    {
        List<Vector3> allGoblinPos = new List<Vector3>();
        (float dist, int index) smallestValueIndex = (100f, 0);
        int iter = 0;
        nmp_checkingPath = new NavMeshPath();
        // Collect all active goblin positions
        foreach(Goblin gob in FindObjectsOfType<Goblin>())
        {
            if (!gob.enabled)
                continue;
            if (NavMesh.CalculatePath(transform.position, gob.transform.position, -1, nmp_checkingPath))
                allGoblinPos.Add(gob.transform.position);
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
            nmp_followingPath = nmp_checkingPath;
            return true;
        }
        return false;
    }

    private bool AbleToAttack()
    {
        //if (Vector3.Distance(nma_self.destination, transform.position) < 1f)
        //    return true;
        return false;
    }

    private bool FindInteractable()
    {
        return false;
    }

    public IEnumerator AttackHitbox()
    {
        yield return new WaitForSeconds(0.5f);
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
