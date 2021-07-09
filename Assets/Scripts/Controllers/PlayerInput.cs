
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : BaseInput
{

    // Update is called once per frame
    void Update()
    {
        v_movement.x = Input.GetAxis("Horizontal");
        v_movement.y = Input.GetAxis("Jump");
        v_movement.z = Input.GetAxis("Vertical");
        v_looking.x = Input.mousePosition.x;
        v_looking.y = Input.mousePosition.y;
        v_looking.z = Input.mouseScrollDelta.x;
        v_action.x = Input.GetAxis("Fire1");
        v_action.y = Input.GetAxis("Fire2");
        v_action.z = Input.GetAxis("Fire3");
        f_special = Input.GetAxis("Submit");
    }
}
