using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Camera main;
    public Camera mini;
 
    void Start()
    {
        main.enabled = true;
        mini.enabled = false;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            main.enabled = !main.enabled;
            mini.enabled = !mini.enabled;
        }
    }
}
