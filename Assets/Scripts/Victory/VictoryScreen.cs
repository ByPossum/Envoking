using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] Text txt_groupText;
    private void Start()
    {
#if GROUPA
        txt_groupText.text += "A";
#else
        txt_groupText.text += "B";
#endif
    }
    public void BackToStart()
    {
        SceneManager.LoadScene("ConsentForm");
    }
}
