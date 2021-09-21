using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpHitBox : MonoBehaviour
{
    [SerializeField] PlayerController owner;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            owner.SetCanJump = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            owner.SetCanJump = false;
            Debug.Log("Can't Jump");
        }
    }
}
