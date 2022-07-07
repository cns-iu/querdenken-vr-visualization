using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [Header("Entities")]
    [SerializeField] private List<GameObject> GroupSymbols = new List<GameObject>();
    [SerializeField] private List<GameObject> ChannelSymbols = new List<GameObject>();
    [SerializeField] private List<GameObject> EdgeObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> NodeObjects = new List<GameObject>();
    [SerializeField] private List<Node> Nodes;
    [SerializeField] private List<Edge> Edges;

    [Header("Scene Setup")]
    [SerializeField] private GameObject NodeParent;
    [SerializeField] private GameObject EdgeParent;
    [SerializeField] private Vector3 offset = new Vector3(-1f, 2f, -3f);
    [SerializeField] private DataReader DataReader;
    [SerializeField] private float scalingFactor = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject pre_Node;
    [SerializeField] private GameObject pre_Edge;

    [Header("Visual Encoding")]
    [SerializeField] private Color Blue;
    [SerializeField] private Color Pink;
    [SerializeField] private float MaxEdgeWidth;
    [SerializeField] private float MinEdgeWidth;
    [SerializeField] private Color MaxColor;
    [SerializeField] private Color MinColor;
    [SerializeField] private float MinSize;
    [SerializeField] private float MaxSize;


    void Start()
    {
        GetLists();
        CreateNodes();
        LayOutNodes(NodeObjects);
        OffsetNodes(offset);
        CreateEdges();
        ForNodesAndEdgesFillConnectionProperties();
        SetEdgePositionsandWidth();
        SizeNodes();
    }

    void CreateEdges()
    {
        foreach (var edge in Edges)
        {
            GameObject line = Instantiate(pre_Edge);
            EdgeData data = line.AddComponent<EdgeData>();
            data.Source = edge.sourceID;
            data.Target = edge.targetID;
            data.Weight = edge.weight;

            EdgeObjects.Add(line);
        }
    }

    void SetEdgePositionsandWidth()
    {
        for (int i = 0; i < EdgeObjects.Count; i++)
        {
            GameObject line = EdgeObjects[i];
            EdgeData data = line.GetComponent<EdgeData>();
            EdgeObjects[i].GetComponent<LineRenderer>().SetPositions(
                new Vector3[2]{
                data.SourceNode.transform.position,
                data.TargetNode.transform.position
        }
            );

            float maxWeight = GetWeights(Edges);
            float width = Mathf.Lerp(MinEdgeWidth, MaxEdgeWidth, data.Weight / maxWeight);
            line.GetComponent<LineRenderer>().startWidth = width;
            line.GetComponent<LineRenderer>().endWidth = width;

            line.GetComponent<LineRenderer>().startColor = Color.Lerp(MinColor, MaxColor, data.Weight / maxWeight);
            line.GetComponent<LineRenderer>().endColor = Color.Lerp(MinColor, MaxColor, data.Weight / maxWeight);

            line.transform.parent = EdgeParent.transform;
        }
    }

    void GetLists()
    {
        Nodes = DataReader.Nodes;
        Edges = DataReader.Edges;
    }

    float GetWeights(List<Edge> collection)
    {
        List<float> weights = new List<float>();
        foreach (var item in collection)
        {
            weights.Add(item.weight);
        }

        return Mathf.Max(weights.ToArray());
    }

    void ForNodesAndEdgesFillConnectionProperties()
    {
        for (int i = 0; i < NodeObjects.Count; i++)
        {
            NodeData n = NodeObjects[i].GetComponent<NodeData>();
            for (int k = 0; k < EdgeObjects.Count; k++)
            {
                EdgeData e = EdgeObjects[k].GetComponent<EdgeData>();
                // Debug.Log("Edge source is " + e.Source + " and Node ID is " + n.Id + " and they are equal: " + e.Source == n.Id);
                if (e.Source == n.Id)
                {
                    n.OutgoingEdges.Add(e.gameObject);
                    e.SourceNode = n.gameObject;
                }

                if (e.Target == n.Id)
                {
                    n.IncomingEdges.Add(e.gameObject);
                    e.TargetNode = n.gameObject;
                };
            }
        }
    }

    void LayOutNodes(List<GameObject> symbols)
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            symbols[i].transform.position = symbols[i].GetComponent<NodeData>().Position * scalingFactor;
        }
    }

    void OffsetNodes(Vector3 offset)
    {
        EdgeParent.transform.Translate(offset);
        NodeParent.transform.Translate(offset);
    }

    void CreateNodes()
    {
        foreach (var node in Nodes)
        {
            GameObject mark = Instantiate(pre_Node);
            NodeData data = mark.AddComponent<NodeData>();
            data.Id = node.Id;
            data.EntityType = node.EntityType;
            data.Position = node.Position;

            mark.transform.parent = NodeParent.transform;

            if (node.EntityType == "Group")
            {
                mark.GetComponent<Renderer>().material.color = Blue;
                GroupSymbols.Add(mark);
            }
            else
            {
                mark.GetComponent<Renderer>().material.color = Pink;
                ChannelSymbols.Add(mark);
            }

            NodeObjects.Add(mark);
        }
    }

    void SizeNodes()
    {
        List<float> degrees = new List<float>();
        foreach (var item in NodeObjects)
        {
            degrees.Add(item.GetComponent<NodeData>().IncomingEdges.Count + item.GetComponent<NodeData>().OutgoingEdges.Count);
        }
        float max = Mathf.Max(degrees.ToArray());

        foreach (var item in NodeObjects)
        {
            item.gameObject.transform.localScale = new Vector3(
                Mathf.Lerp(MinSize, MaxSize, item.GetComponent<NodeData>().IncomingEdges.Count / max),
                Mathf.Lerp(MinSize, MaxSize, item.GetComponent<NodeData>().IncomingEdges.Count / max),
                Mathf.Lerp(MinSize, MaxSize, item.GetComponent<NodeData>().IncomingEdges.Count / max)
            );
        }
    }
}
