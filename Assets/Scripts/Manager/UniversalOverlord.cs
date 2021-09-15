using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalOverlord : MonoBehaviour
{
    public static UniversalOverlord x;
    [SerializeField] private Manager[] A_managers;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Manager man in A_managers)
            man.Init();

        if (x != null)
            Destroy(this);
        else
        {
            x = this;
            DontDestroyOnLoad(this);
        }
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
}
