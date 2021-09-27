using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollider : MonoBehaviour
{
    private const float f_DAMAGE = 0.5f;
    private const float f_TIME = 0.5f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<PlayerController>())
        {
            collision.collider.GetComponent<PlayerController>().LavaDamage(f_DAMAGE, f_TIME);
        }
    }
}
