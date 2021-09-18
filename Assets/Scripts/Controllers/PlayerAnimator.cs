using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator a_playerAnim;

    public void SetAnimBool(string _boolName, bool _boolState)
    {
        a_playerAnim.SetBool(_boolName, _boolState);
    }

    public void SetAnimFloat(string _floatName, float _value)
    {
        a_playerAnim.SetFloat(_floatName, _value);
    }

    public void SetAnimTrigger(string _triggerName)
    {
        a_playerAnim.SetTrigger(_triggerName);
    }

    public void ResetAnimValues()
    {
        // Find a better way to cycle through things to turn them false
        a_playerAnim.SetBool("Jump", false);
        a_playerAnim.SetBool("Falling", false);
        a_playerAnim.SetBool("OnGround", true);
        a_playerAnim.SetBool("Killed", false);
        a_playerAnim.SetBool("Pet", false);
        a_playerAnim.SetBool("Carry", false);
        a_playerAnim.SetBool("Throw", false);
        a_playerAnim.SetFloat("WalkSpeed", 0.0f);
        //a_playerAnim.SetBool("", false);
    }

    public void SetAnimLayerWeight(int _layerID, float _layerWeight)
    {
        a_playerAnim.SetLayerWeight(_layerID, _layerWeight);
    }
}
