﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInput : MonoBehaviour
{
    protected Vector3 v_movement;
    protected Vector3 v_looking;
    protected Vector3 v_action;
    protected float f_special;
    protected bool b_jump;
    public Vector3 Movement { get { return v_movement; } }
    public Vector3 Look { get { return v_looking; } }
    public Vector3 Actions { get { return v_action; } } 
    public float Special { get { return f_special; } }
    public bool Jump { get { return b_jump; } }
}
