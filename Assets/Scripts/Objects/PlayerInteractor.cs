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
        FindObjectOfType<Dog>().transform.position = FindObjectOfType<PlayerController>().transform.position + Utils.RandomVector3(1f, true);
        base.Interact();
    }

}
