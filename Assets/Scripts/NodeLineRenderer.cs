using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLineRenderer : MonoBehaviour
{
    private LineRenderer lr;
    public GameMap gamemap;
    public float lineThickness;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        ConnectAllNodes();
        
    }


    public void ConnectAllNodes()
    {
        List<Vector3> nodesList = new List<Vector3>();
        foreach (Node node in gamemap.mapNodes)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if(neighbour.visitedByLineRenderer == false)
                {
                    nodesList.Add(neighbour.transform.position);
                    nodesList.Add(node.transform.position);
                }
                else
                {
                    
                }
            }
            node.visitedByLineRenderer = true;
        }
        
        lr.positionCount = nodesList.Count;
        lr.SetPositions(nodesList.ToArray());
        lr.useWorldSpace = true;
        lr.startWidth = lineThickness;
        lr.endWidth = lineThickness;
        lr.startColor = Color.black;
        lr.endColor = Color.black;
    }

    public void ConnectNodes(Node x, Node y)
    {
        List<Vector3> nodes = new List<Vector3>();
        nodes.Add(x.transform.position);
        nodes.Add(y.transform.position);
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPositions(nodes.ToArray());
        lr.useWorldSpace = true;
    }
}
