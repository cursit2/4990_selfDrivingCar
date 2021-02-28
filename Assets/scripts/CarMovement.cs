using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardSpeed = 3f, reverseSpeed = 1f, maxSpeed = 20f, turnStrength = 90f, speedInput, turnInput;

    private void Start()
    {
        rb.transform.parent = null;
    }
    private void Update()
    {
        speedInput = 0f;
        if(Input.GetAxis("Vertical") > 0)
        {
            MoveForward();
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            MoveBackward();
        }

        turnInput = Input.GetAxis("Horizontal");
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime, 0f));



       // transform.position = rb.transform.position;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(speedInput) > 0)
        {
            rb.AddForce(transform.forward * speedInput);
        }
        
    }

    public void MoveForward()
    {
        speedInput = Input.GetAxis("Vertical") * forwardSpeed * 1000f;
        transform.position = rb.transform.position;
    }
    public void MoveBackward()
    {
        speedInput = Input.GetAxis("Vertical") * reverseSpeed * 1000f;
        transform.position = rb.transform.position;
    }
    public void MoveLeft()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, -1f * turnStrength * Time.deltaTime, 0f));
        transform.position = rb.transform.position;
    }
    public void MoveRight()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 1f * turnStrength * Time.deltaTime, 0f));
        transform.position = rb.transform.position;
    }
}
