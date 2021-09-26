using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalOverlord : MonoBehaviour
{
    public static UniversalOverlord x;
    [SerializeField] private Manager[] A_managers;
    // Start is called before the first frame update
    void Start()
    {

        x = this;

        RefreashManagers();
    }

    private void RefreashManagers()
    {
        foreach (Manager man in A_managers)
        {
            man.Init();
        }
    }

    public void SceneInit(Scene _restart, LoadSceneMode _mode)
    {
        RefreashManagers();
    }

    public T GetManager<T>(ManagerTypes _managerToGet) where T : Manager
    {
        switch (_managerToGet)
        {
            case ManagerTypes.PoolManager:
                return (T)A_managers[(int)_managerToGet-1];
            default:
                return null;
        }
    }

    // Figure out a nicer way to do this
    public Manager GetManager(int _id)
    {
        return A_managers[_id];
    }
    public void ClearManagers()
    {
        GetManager<PoolManager>(ManagerTypes.PoolManager).DestroyPools();
    }
}
