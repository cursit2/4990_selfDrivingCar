using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivePath : MonoBehaviour
{
    public Path path;
    public Localization localization;
    public Transform car;
    public CarMovement movement;
    public float rotationSpeed = 2f;
    private List<Transform> pathNodes;
    private Vector3 direction;
    private Quaternion lookRotation;

    private void Update()
    {
        pathNodes = path.GetNodes();
    }

    void FixedUpdate()
    {

        if (pathNodes.Count >= 1) {
            //USING POSITION FOUND FROM LOCALIZATION
            direction = (pathNodes[0].position - localization.truePosition);
            lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        else return;
    }
}
