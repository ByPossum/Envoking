using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Manager
{
    [SerializeField] private PoolObjectQuantity[] poq_poolsToGenerate;
    private Dictionary<string, Pool> D_pools = new Dictionary<string, Pool>();
    /// <summary>
    /// Creates pools and populates them
    /// </summary>
    public override void Init()
    {
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
    /// <summary>
    /// Sets object active in scene and positions it at set location.
    /// </summary>
    /// <param name="_poolName">Pool to get object from</param>
    /// <param name="_start">Position to start the object</param>
    /// <returns>Object being spawned</returns>
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

    /// <summary>
    /// Sets object active in scene and positions it at set location and rotation
    /// </summary>
    /// <param name="_poolName">Pool to get object from</param>
    /// <param name="_start">Position to start the object</param>
    /// <param name="_rotation">Rotation to start the object</param>
    /// <returns>Object being spawned</returns>
    public GameObject SpawnObject(string _poolName, Vector3 _start, Quaternion _rotation)
    {
        GameObject obj = SpawnObject(_poolName, _start);
        if (obj == null)
        {
            return null;
        }
        obj.transform.rotation = _rotation;
        return obj;
    }
    /// <summary>
    /// Returns object into pool
    /// </summary>
    /// <param name="_pooledObject">Object to return</param>
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
    public void DestroyPools()
    {
        // Let previous objects be garbage collected
        D_pools.Clear();
        poq_poolsToGenerate = new PoolObjectQuantity[0];
    }
}