using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController pa_controller;
    [SerializeField] private Vector3 v_offset;
    [SerializeField] private Vector3 v_walkingOffset;
    [SerializeField] private float f_cameraSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, pa_controller.transform.position + (pa_controller.CurrentAction == PlayerAction.walking ? v_walkingOffset : v_offset), f_cameraSpeed);
    }
}
