using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEndPos : MonoBehaviour
{
    public GameObject endPos;
    public LayerMask layer;
    private Vector3 newPosition;
    public Camera mini;
    void Start()
    {

    }
    private void Update()
    {
        //if clicked on the road, change end pos to that position
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mini.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, layer))
            {
                newPosition = hit.point;
                newPosition = new Vector3(newPosition.x, 1, newPosition.z);
                endPos.transform.position = newPosition;
            }
        }
    }
}
