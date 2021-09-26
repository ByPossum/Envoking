using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : Interacters, IUsable
{
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public override void Interact()
    {
        GetComponent<Collider>().enabled = false;
        base.Interact();
    }

}
