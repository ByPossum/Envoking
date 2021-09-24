
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : BaseInput
{

    // Update is called once per frame
    void Update()
    {
        v_movement.x = Input.GetAxis("Horizontal");
        v_movement.z = Input.GetAxis("Vertical");
        v_looking.x = Input.mousePosition.x;
        v_looking.y = Input.mousePosition.y;
        v_looking.z = Input.mouseScrollDelta.x;
        v_action.x = Input.GetButtonDown("Fire1") ? 1f : 0f;
        v_action.y = Input.GetButtonDown("Fire2") ? 1f : 0f;
        v_action.z = Input.GetButtonDown("Fire3") ? 1f : 0f;
        b_special = Input.GetButtonDown("Submit");
        b_jump = Input.GetButtonDown("Jump");
    }
    public void NoJump()
    {
        v_movement.y = 0.0f;
    }
}
