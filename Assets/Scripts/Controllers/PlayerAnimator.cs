using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : AnimatorController
{
    public void ResetAnimValues()
    {
        // Find a better way to cycle through things to turn them false
        a_playerAnim.SetBool("Carry", false);
        a_playerAnim.SetFloat("WalkSpeed", 0.0f);
        // Just in case the player is still jumping
        a_playerAnim.SetTrigger("Fall");
        //a_playerAnim.SetBool("", false);
    }
}
