using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySwitcher : MonoBehaviour
{
    public void Awake()
    {
        SceneManager.LoadScene("VictoryScreen");
    }
}
