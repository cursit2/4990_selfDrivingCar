using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivePath : MonoBehaviour
{
    public Localization localization;
    public Transform car;
    public CarMovement movement;
    public float rotationSpeed = 2f;
    private Vector3 direction;
    private Quaternion lookRotation;
    private List<Node> drivePath;
    private Grid gridReference;
    public GameObject manager;


    private void Awake()//When the program starts
    {
        gridReference = manager.GetComponent<Grid>();//Get a reference to the game manager
    }
    void FixedUpdate()
    {
        drivePath = gridReference.FinalPath;
        if (drivePath.Count >= 1) {
            //USING POSITION FOUND FROM LOCALIZATION
            direction = ((drivePath[0].vPosition + new Vector3(0, 0.5f, 0)) - localization.truePosition);
            lookRotation = Quaternion.LookRotation(direction);


            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        }
        else return;
    }
    
}
