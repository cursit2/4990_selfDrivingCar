using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracing : MonoBehaviour
{

    RaycastHit hitinfo;
    public float raydistance = 4f;
    public CarMovement movement;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.right, out hitinfo, raydistance))
        {
           // movement.MoveLeft();
        }
        if (Physics.Raycast(transform.position, -transform.right, out hitinfo, raydistance))
        {
           // movement.MoveRight();
        }




        Debug.DrawRay(transform.position, transform.forward * 4);
        Debug.DrawRay(transform.position, transform.right * 4);
        //Debug.DrawRay(transform.position, -transform.right * 4);
    }

}
