using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacters : MonoBehaviour
{
    [SerializeField] protected List<GameObject> L_turnOn = new List<GameObject>();
    [SerializeField] protected List<GameObject> L_turnOff = new List<GameObject>();

    public virtual void Interact()
    {
        if (L_turnOn.Count > 0)
            foreach (GameObject go_on in L_turnOn)
                go_on.SetActive(true);
        if (L_turnOff.Count > 0)
            foreach (GameObject go_off in L_turnOff)
                go_off.SetActive(false);
        Destroy(this);
    }
}
