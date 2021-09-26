using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            other.GetComponent<PlayerController>().SpawnPoint = transform.position;
        }
        else if (other.GetComponent<Dog>())
        {
            other.GetComponent<Dog>().RespawnPoint = transform.position;
        }
    }
}
