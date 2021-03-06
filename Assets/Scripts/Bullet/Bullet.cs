using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float f_damage;
    [SerializeField] private float f_lifetime;
    [SerializeField] private BaseInput bi_owner;
    [SerializeField] private LayerMask lm_ignoreLayer;

    public void Start()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, Utils.LMToInt(lm_ignoreLayer));
    }

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
                    Die();
                    break;
                case Creature npc:
                    npc.TakeDamage(f_damage);
                    Die();
                    break;
                default:
                    break;
            }
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            Die();
        }

    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(f_lifetime);
        Die();
    }

    private void Die()
    {
        UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).ReturnToPool(gameObject);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
