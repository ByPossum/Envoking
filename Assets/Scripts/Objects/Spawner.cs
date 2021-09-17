using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int i_totalNumber;
    [SerializeField] private int i_minAliveAgents;
    [SerializeField] private GameObject go_monsterType;
    [SerializeField] private List<GameObject> L_turnOn;
    [SerializeField] private List<GameObject> L_turnOff;
    // Update is called once per frame
    void Update()
    {
        if (!CheckAliveMonsters())
            SpawnNewMonster();
    }

    private bool CheckAliveMonsters()
    {
        if (UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).GetActiveObjectsInPool(go_monsterType) < i_minAliveAgents)
            return false;
        return true;
    }
    private void SpawnNewMonster()
    {
        if (i_totalNumber <= 0)
            Die();
        UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).SpawnObject(go_monsterType.name, transform.position);
        i_totalNumber--;
    }

    private void Die()
    {
        if (L_turnOn.Count > 0)
            foreach (GameObject go_on in L_turnOn)
                go_on.SetActive(true);
        if (L_turnOff.Count > 0)
            foreach (GameObject go_off in L_turnOff)
                go_off.SetActive(false);
        Destroy(this);
    }
}
