using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsentFormManager : MonoBehaviour
{
    [SerializeField] private string s_sceneName;
    public void Consent()
    {
        SceneManager.LoadScene(s_sceneName);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
