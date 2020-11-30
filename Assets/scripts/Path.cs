using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Path : MonoBehaviour
{
    public Color lineColor;
    private List<Transform> nodes;
    public Transform car;
    private Vector3 carNode;
    public float distance;
    public float distanceThreshhold = 5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Vector3 previousNode = car.position;

        nodes = GetNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Gizmos.DrawWireSphere(currentNode, 2);
            if (i >= 1)
            {
                previousNode = nodes[i - 1].position;
            }
            Gizmos.DrawLine(previousNode, currentNode);
        }
    }

    private void Update()
    {
        nodes = GetNodes();
        carNode = car.position;
        if (nodes.Count >= 1)
        {
            distance = (carNode - nodes[0].position).magnitude;
            if (distance < distanceThreshhold)
            {
                Destroy(nodes[0].gameObject);
            }
        }
    }

    public List<Transform> GetNodes()
    {
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        return nodes;
    }
}
