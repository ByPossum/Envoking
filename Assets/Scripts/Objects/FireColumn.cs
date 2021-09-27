using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireColumn : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Flicker", Random.Range(0.5f, 4.5f), Random.Range(0.5f, 4.5f));
    }

    private void Flicker()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
