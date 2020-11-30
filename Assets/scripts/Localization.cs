using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public Transform car;
    public Transform[] cellTowers;
    public float rayDistance = 10f;
    private RaycastHit hitInfo;
    public LayerMask layer;
    private Vector3 startVectorleft;
    private Vector3 endVectorleft;
    private Vector3 startVectorRight;
    private Vector3 endVectorRight;
    private Vector3 currentVectorleft;
    private Vector3 currentVectorRight;
    
    private Vector3 firstHitleft;
    private float firstHitDistanceleft;
    private Vector3 lastHitleft;
    private float lastHitDistanceleft;
    private Vector3 firstHitRight;
    private float firstHitDistanceRight;
    private Vector3 lastHitRight;
    private float lastHitDistanceRight;

    private Vector3 firstHit;
    public float firstHitDistance;
    private Vector3 lastHit;
    public float lastHitDistance;

    public Vector3 GPSguess;
    public float GPSguessDistance;
    public float DistanceToKnownPoint;
    private float errorRange = 2.5f;
    private bool gotFirstHitleft = false;
    private bool gotFirstHitRight = false;

    private float A, B, C, D, E, F;
    private float xValue;
    private float zValue;

    public Vector3 truePosition;

    private void Start()
    {
        GPSGuesser();
        //get gps guess within error range. repeat at a reasonable rate
        InvokeRepeating("GPSGuesser", 0.4f, 0.4f);
    }
    void Update()
    {
        //making a cone of raycast to find first and last point for trianglization. Left side of car
        startVectorleft = Quaternion.AngleAxis(-45f, car.up) * (-car.right);
        endVectorleft = Quaternion.AngleAxis(45f, car.up) * (-car.right);
        Debug.DrawRay(car.position, startVectorleft * rayDistance, Color.red);
        Debug.DrawRay(car.position, endVectorleft * rayDistance, Color.blue);
        //make cone on right side
        startVectorRight = Quaternion.AngleAxis(135f, car.up) * (-car.right);
        endVectorRight = Quaternion.AngleAxis(225f, car.up) * (-car.right);
        Debug.DrawRay(car.position, startVectorRight * rayDistance, Color.red);
        Debug.DrawRay(car.position, endVectorRight * rayDistance, Color.blue);

        currentVectorleft = startVectorleft;
        //first hit on the building. find furthest apart first hit and last hit.
        gotFirstHitleft = false;
        gotFirstHitRight = false;

        //left side
        for (int i = 0; i < 90; i++)
        {
            currentVectorleft = Quaternion.AngleAxis((-45 + i), car.up) * (-car.right);
            if(Physics.Raycast(car.position, currentVectorleft, out hitInfo, rayDistance, layer) && !gotFirstHitleft)
            {
                firstHitleft = hitInfo.point;
                firstHitDistanceleft = hitInfo.distance;
                gotFirstHitleft = true;
            }
            if(Physics.Raycast(car.position, currentVectorleft, out hitInfo, rayDistance, layer)){
                lastHitleft = hitInfo.point;
                lastHitDistanceleft = hitInfo.distance;
            }
        }

        //right side
        currentVectorRight = startVectorRight;
        for (int i = 0; i < 90; i++)
        {
            currentVectorRight = Quaternion.AngleAxis((135 + i), car.up) * (-car.right);
            if (Physics.Raycast(car.position, currentVectorRight, out hitInfo, rayDistance, layer) && !gotFirstHitRight)
            {
                firstHitRight = hitInfo.point;
                firstHitDistanceRight = hitInfo.distance;
                gotFirstHitRight = true;
            }
            if (Physics.Raycast(car.position, currentVectorRight, out hitInfo, rayDistance, layer))
            {
                lastHitRight = hitInfo.point;
                lastHitDistanceRight = hitInfo.distance;
            }
        }

        if (gotFirstHitleft) //if left succeeds then use for calculations
        {
            firstHit = firstHitleft;
            firstHitDistance = firstHitDistanceleft;
            lastHit = lastHitleft;
            lastHitDistance = lastHitDistanceleft;
        }
        else if (gotFirstHitRight) //if left fails and right succeeds use right for calculations
        {
            firstHit = firstHitRight;
            firstHitDistance = firstHitDistanceRight;
            lastHit = lastHitRight;
            lastHitDistance = lastHitDistanceRight;
        }
        else //use failsafe localization method using the cell towers
        {
            firstHit = cellTowers[0].position;
            firstHitDistance = Vector3.Distance(cellTowers[0].position, car.position);
            lastHit = cellTowers[1].position;
            lastHitDistance = Vector3.Distance(cellTowers[1].position, car.position);

            for (int i=1; i < cellTowers.Length; i++) //find two closest
            {
                if ((Vector3.Distance(cellTowers[i].position, car.position)) < firstHitDistance)
                {
                    firstHit = cellTowers[i].position;
                    firstHitDistance = Vector3.Distance(cellTowers[i].position, car.position);
                    lastHit = cellTowers[i - 1].position;
                    lastHitDistance = Vector3.Distance(cellTowers[i - 1].position, car.position);
                }
                else
                {
                    lastHit = cellTowers[i].position;
                    lastHitDistance = Vector3.Distance(cellTowers[i].position, car.position);
                }
            }
        }

        //get distance from guess to car
        DistanceToKnownPoint = Mathf.Sqrt(Mathf.Pow((GPSguess.x - firstHit.x),2) + Mathf.Pow((GPSguess.z - firstHit.z),2));
        if (DistanceToKnownPoint > firstHitDistance)
        {
            GPSguessDistance = Mathf.Sqrt(Mathf.Pow(DistanceToKnownPoint, 2) - Mathf.Pow(firstHitDistance, 2));
        }
        else
        {
            GPSguessDistance = Mathf.Sqrt(Mathf.Pow(firstHitDistance, 2) - Mathf.Pow(DistanceToKnownPoint, 2));
        }

        /*using our 3 points and 3 distances, we can use trilateration
        //Ax + By = C and Dx + Ey = F.   (y = z)
          A = 2*x2 - 2*x1
          B = 2*y2 - 2*y1
          C = r1**2 - r2**2 - x1**2 + x2**2 - y1**2 + y2**2
          D = 2*x3 - 2*x2
          E = 2*y3 - 2*y2
          F = r2**2 - r3**2 - x2**2 + x3**2 - y2**2 + y3**2
          x = (C*E - F*B) / (E*A - B*D)
          y = (C*D - A*F) / (B*D - A*E)
         */
        A = (2 * lastHit.x) - (2 * firstHit.x);
        B = (2 * lastHit.z) - (2 * firstHit.z);
        C = Mathf.Pow(firstHitDistance, 2) - Mathf.Pow(lastHitDistance, 2) - Mathf.Pow(firstHit.x, 2) + Mathf.Pow(lastHit.x, 2) - Mathf.Pow(firstHit.z, 2) + Mathf.Pow(lastHit.z, 2);
        D = (2 * GPSguess.x) - (2 * lastHit.x);
        E = (2 * GPSguess.z) - (2 * lastHit.z);
        F = Mathf.Pow(lastHitDistance, 2) - Mathf.Pow(GPSguessDistance, 2) - Mathf.Pow(lastHit.x, 2) + Mathf.Pow(GPSguess.x, 2) - Mathf.Pow(lastHit.z, 2) + Mathf.Pow(GPSguess.z, 2);
        xValue = ((C * E) - (F * B)) / ((E * A) - (B * D));
        zValue = ((C * D) - (A * F)) / ((B * D) - (A * E));

        truePosition = new Vector3(xValue, car.position.y, zValue);
    }
    private void GPSGuesser()
    {
        GPSguess = new Vector3(car.position.x + UnityEngine.Random.Range(-errorRange, errorRange), car.position.y, car.position.z + UnityEngine.Random.Range(-errorRange, errorRange));
    }
}
