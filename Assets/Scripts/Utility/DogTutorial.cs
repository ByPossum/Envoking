using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if GROUPA
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
    }
}
