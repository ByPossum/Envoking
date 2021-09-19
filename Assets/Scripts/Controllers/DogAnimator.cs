using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimator : AnimatorController
{
    public void ResetAnimParameters()
    {
        a_playerAnim.SetFloat("Walk", 0f);
    }
}
