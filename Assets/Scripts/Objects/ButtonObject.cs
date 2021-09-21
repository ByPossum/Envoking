using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : MonoBehaviour, IDogable, IUsable
{
    [SerializeField] private List<GameObject> L_turnOn = new List<GameObject>();
    [SerializeField] private List<GameObject> L_turnOff = new List<GameObject>();

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Interact()
    {
        if(L_turnOn.Count > 0)
            foreach (GameObject go_on in L_turnOn)
                go_on.SetActive(true);
        if(L_turnOff.Count > 0)
            foreach (GameObject go_off in L_turnOff)
                go_off.SetActive(false);
        transform.position = new Vector3(transform.position.x, -0.4f, transform.position.z);
        Destroy(this);
    }
}
