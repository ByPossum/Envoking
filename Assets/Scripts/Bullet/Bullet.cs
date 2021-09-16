using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float f_damage;
    [SerializeField] private float f_lifetime;
    [SerializeField] private BaseInput bi_owner;

    public void SetOwner(BaseInput _newOwner)
    {
        StopAllCoroutines();
        bi_owner = _newOwner;
        StartCoroutine(DeathTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<BaseInput>() && collision.gameObject.GetComponent<BaseInput>() != bi_owner)
        {
            BaseInput bi = collision.gameObject.GetComponent<BaseInput>();
            switch (bi)
            {
                case PlayerInput player:
                    player.GetComponent<PlayerController>().TakeDamage(f_damage);
                    UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).ReturnToPool(gameObject);
                    break;
                case Creature npc:
                    npc.TakeDamage(f_damage);
                    UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).ReturnToPool(gameObject);
                    break;
                default:
                    break;
            }
        }
        else if (collision.transform.GetComponent<Bullet>())
        {
            UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).ReturnToPool(gameObject);
        }

    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(f_lifetime);
        UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).ReturnToPool(gameObject);
    }
}
