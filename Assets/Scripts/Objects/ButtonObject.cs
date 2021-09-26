﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : Interacters, IDogable, IUsable
{


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public override void Interact()
    {
        transform.position = new Vector3(transform.position.x, -0.4f, transform.position.z);
        base.Interact();
    }

}
