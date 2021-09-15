using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    public virtual void Spawn(Vector3 _pos)
    {
        gameObject.SetActive(true);
        transform.position = _pos;
    }

    public virtual void Spawn(Vector3 _pos, Quaternion _rot)
    {
        Spawn(_pos);
        transform.rotation = _rot;
    }

    public virtual void Spawn(Vector3 _pos, Quaternion _rot, Vector3 _direction, float _force)
    {
        Spawn(_pos, _rot);
        transform.GetComponent<Rigidbody>().AddForce(_direction * _force, ForceMode.Impulse);
    }

    public virtual void Die(Vector3 _pos)
    {
        gameObject.SetActive(false);
        transform.position = _pos;
    }
}
