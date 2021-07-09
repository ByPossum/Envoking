using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BaseInput))]
public class Controller : MonoBehaviour
{
    protected BaseInput bi_input;
    // Start is called before the first frame update
    void Start()
    {
        bi_input = GetComponent<BaseInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
