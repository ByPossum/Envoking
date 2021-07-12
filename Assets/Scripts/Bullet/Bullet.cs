using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float f_damage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Goblin>())
        {
            collision.transform.GetComponent<Goblin>().TakeDamage(f_damage);
            Destroy(gameObject);
        }
    }
}
