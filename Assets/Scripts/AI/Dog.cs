using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dog : Creature
{
    private DogActions da_currentAction = DogActions.none;
    public DogActions CurrentAction { get { return da_currentAction; } }
    // Update is called once per frame
    void Update()
    {
        CheckActionToPerform();
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
        nma_self.destination = FindObjectOfType<PlayerController>().transform.position;
        if (nma_self.hasPath)
        {
            v_movement = nma_self.destination;
            return true;
        }
        return false;
    }

    private bool FindEnemy()
    {
        List<Vector3> allGoblinPos = new List<Vector3>();
        (float dist, int index) smallestValueIndex = (100f, 0);
        int iter = 0;

        // Collect all active goblin positions
        foreach(Goblin gob in FindObjectsOfType<Goblin>())
        {
            if (!gob.enabled)
                continue;
            nma_self.destination = gob.transform.position;
            if (nma_self.hasPath)
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
        nma_self.destination = allGoblinPos[smallestValueIndex.index];
        v_movement = nma_self.destination;
        return true;
    }

    private bool AbleToAttack()
    {
        if (Vector3.Distance(nma_self.destination, transform.position) < 1f)
            return true;
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
