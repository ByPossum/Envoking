using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController pa_controller;
    [SerializeField] private Vector3 v_offset;
    [SerializeField] private Vector3 v_walkingOffset;
    [SerializeField] private Slider sl_healthBar;
    [SerializeField] private float f_cameraSpeed;
    [SerializeField] private GameObject go_deathMask;
    [SerializeField] private GameObject go_buttons;

    // Update is called once per frame
    void Update()
    {
        // Zoom the camera into the correct offset
        transform.position = Vector3.Lerp(transform.position, pa_controller.transform.position + (pa_controller.CurrentAction == PlayerAction.walking ? v_walkingOffset : v_offset), f_cameraSpeed);
        // Show death screen when player dies
        if (pa_controller.CurrentAction == PlayerAction.gameOver)
        {
            go_deathMask.SetActive(true);
            go_buttons.SetActive(true);
        }
        // Update healthbar
        sl_healthBar.value = pa_controller.CurrentHealth / pa_controller.MaxHealth;
    }

    /// <summary>
    /// Called from button. Refreshes active scene.
    /// </summary>
    public void RestartGame()
    {
        UniversalOverlord.x.ClearManagers();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// Called from button. Returns to consent form.
    /// </summary>
    public void QuitGame()
    {
        SceneManager.LoadScene("ConsentForm");
        Time.timeScale = 1f;
    }
}
