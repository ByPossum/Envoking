using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Manager
{
    [SerializeField] private PoolObjectQuantity[] poq_poolsToGenerate;
    private Dictionary<string, Pool> D_pools = new Dictionary<string, Pool>();
    // Start is called before the first frame update
    public override void Init()
    {
        DontDestroyOnLoad(this);
        foreach (PoolObjectQuantity poolable in poq_poolsToGenerate)
        {
            if (poolable.go_poolable.GetComponent<Poolable>() != null || poolable.go_poolable.GetComponentInChildren<Poolable>() != null)
            {
                Pool newPool = new Pool();
                newPool.InitAllObjects(poolable.go_poolable, poolable.i_quantity);
                D_pools.Add(poolable.go_poolable.name, newPool);
            }
        }
    }

    public GameObject SpawnObject(string _poolName, Vector3 _start)
    {
        foreach(string poolNames in D_pools.Keys)
        {
            if (poolNames.Equals(_poolName))
            {
                Pool obtainedPool = D_pools[poolNames];
                GameObject obj = obtainedPool.GetNextAvailableObject().gameObject;
                obj.SetActive(true);
                obj.transform.position = _start;
                return obj;
            }
        }
        return null;
    }

    public GameObject SpawnObject(string _poolName, Vector3 _start, Quaternion _rotation)
    {
        GameObject obj = SpawnObject(_poolName, _start);
        if (obj == null)
        {
            Debug.Log("Not returning object");
            return null;
        }
        obj.transform.rotation = _rotation;
        return obj;
    }

    public void ReturnToPool(GameObject _pooledObject)
    {
        Pool obtainedPool = D_pools[_pooledObject.name];
        obtainedPool.KillObject(_pooledObject);
    }

    public int GetActiveObjectsInPool(GameObject _poolType)
    {
        return D_pools[_poolType.name].ObjectCount();
    }
    public Poolable[] GetObjectCollection(string _objectName)
    {
        return D_pools[_objectName].GetPoolables();
    }
    public Poolable[] GetObjectCollection(GameObject _pooledObject)
    {
        return GetObjectCollection(_pooledObject.name);
    }
}