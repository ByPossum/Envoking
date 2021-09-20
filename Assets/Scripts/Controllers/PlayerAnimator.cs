﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : AnimatorController
{
    public void ResetAnimValues()
    {
        // Find a better way to cycle through things to turn them false
        a_playerAnim.SetBool("Jump", false);
        a_playerAnim.SetBool("Falling", false);
        a_playerAnim.SetBool("Killed", false);
        a_playerAnim.SetBool("Pet", false);
        a_playerAnim.SetBool("Carry", false);
        a_playerAnim.SetBool("Throw", false);
        a_playerAnim.SetFloat("WalkSpeed", 0.0f);
        //a_playerAnim.SetBool("", false);
    }
}