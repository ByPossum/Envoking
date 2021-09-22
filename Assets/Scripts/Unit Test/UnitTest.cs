using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTest : MonoBehaviour
{
    private bool b_success;

    private void Start()
    {
        StartCoroutine(StartTimer());
    }

    private bool InitialiseTest()
    {
        Poolable[] allBullets = UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager).GetObjectCollection("Bullet");
        return allBullets.Length == 20;
    }

    private bool SpawnTest()
    {
        int bulletCount = 0;
        PoolManager pm = UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager);
        pm.SpawnObject("Bullet", transform.position);
        Poolable[] allBullets = pm.GetObjectCollection("Bullet");
        foreach (Poolable bulletCheck in allBullets)
            if(bulletCheck.isActiveAndEnabled)
                bulletCount++;
        return bulletCount == 1;
    }

    private bool ResizeTest()
    {
        PoolManager pm = UniversalOverlord.x.GetManager<PoolManager>(ManagerTypes.PoolManager);
        Poolable[] allBullets = pm.GetObjectCollection("Bullet");
        int previousSize = allBullets.Length;
        for (int i = 0; i < previousSize; i++)
            pm.SpawnObject("Bullet", transform.position + Utils.RandomVector3(30, true));
        return pm.GetObjectCollection("Bullet").Length == previousSize + 1;
    }

    private void PrintTestResults(string _testName, bool _testResults)
    {
        Debug.Log(string.Format("{0} {1} {2}", _testName, System.DateTime.Now, _testResults));

    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(2f);
        PrintTestResults("Initialising Pool", InitialiseTest());
        PrintTestResults("Spawning One Object", SpawnTest());
        PrintTestResults("Resizing Pool at Runtime", ResizeTest());
        Debug.Log("Done");
    }
}
