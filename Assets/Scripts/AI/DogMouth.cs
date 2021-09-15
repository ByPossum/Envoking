using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMouth : MonoBehaviour
{
    [SerializeField] private Dog dog_owner;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Goblin>() != null)
        {
            Goblin monsterAttacked = other.GetComponent<Goblin>();
            monsterAttacked.TakeDamage(dog_owner.AttackDamage);
            monsterAttacked.Knockback(Vector3.Scale(monsterAttacked.transform.position - transform.position, Vector3.one - Vector3.up).normalized, dog_owner.AttackForce);
        }
    }
}
