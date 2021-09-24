using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private Poolable[] pool_objects;
    private GameObject poolType;

    public void InitAllObjects(GameObject _poolType, int _poolSize)
    {
        pool_objects = new Poolable[_poolSize];
        poolType = _poolType;
        for(int i = 0; i < _poolSize; i++)
        {
            GameObject newObj = Instantiate(_poolType);
            newObj.name = newObj.name.Split('(')[0];
            Poolable newPoolable = newObj.GetComponent<Poolable>();
            pool_objects[i] = newPoolable;
            newPoolable.Die(Vector3.zero);
        }
    }

    public Poolable GetNextAvailableObject()
    {
        for(int i = 0; i < pool_objects.Length; i++)
        {
            if (!pool_objects[i].isActiveAndEnabled)
            {
                return pool_objects[i];
            }
        }
        // Replace following with Utils functionality
        // Add new object to the end of the pool
        Poolable[] temp = new Poolable[pool_objects.Length + 1];
        for (int i = 0; i < pool_objects.Length; i++)
            temp[i] = pool_objects[i];
        GameObject newObj = Instantiate(poolType);
        temp[temp.Length-1] = newObj.GetComponent<Poolable>();
        temp[temp.Length-1].name = poolType.name;
        // Reset object
        pool_objects = temp;
        // Send object
        return pool_objects[pool_objects.Length-1];
    }

    public void KillObject(GameObject _tadpole)
    {
        _tadpole.transform.rotation = Quaternion.identity;
        Poolable objToKill = _tadpole.GetComponent<Poolable>();
        if (objToKill != null)
            objToKill.Die(Vector3.zero);
    }

    public void KillAllObjects(Vector3 pos)
    {
        foreach(Poolable dirty in pool_objects)
        {
            dirty.Die(pos);
        }
    }

    public int ObjectCount()
    {
        int activeObjects = 0;
        foreach(Poolable po in pool_objects)
        {
            if (po.isActiveAndEnabled)
                activeObjects++;
        }
        return activeObjects;
    }

    public Poolable[] GetPoolables()
    {
        return pool_objects;
    }
}
