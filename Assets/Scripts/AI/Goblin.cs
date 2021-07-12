using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Creature
{
    private float hp = 5;

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
            Destroy(gameObject);
    }



    public void TakeDamage(float _damage)
    {
        hp -= _damage;
    }
}
