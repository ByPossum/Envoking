using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Creature
{
    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckActionToPerform()
    {

    }
}

public enum DogActions
{
    none,
    follow,
    findPlayer,
    findObstical
}
