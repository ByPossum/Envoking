using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] protected Animator a_playerAnim;

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

    public void SetAnimLayerWeight(int _layerID, float _layerWeight)
    {
        a_playerAnim.SetLayerWeight(_layerID, _layerWeight);
    }
}
