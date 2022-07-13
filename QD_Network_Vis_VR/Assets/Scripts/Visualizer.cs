using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [Header("Entities")]
    [field: SerializeField] public List<GameObject> EdgeObjects = new List<GameObject>();
    [field: SerializeField] public List<GameObject> NodeObjects = new List<GameObject>();
    [field: SerializeField] private List<Node> nodes;
    [field: SerializeField] private List<Edge> edges;
    [field: SerializeField] private List<GameObject> groupSymbols = new List<GameObject>();
    [field: SerializeField] private List<GameObject> channelSymbols = new List<GameObject>();

    [Header("Scene Setup")]
    [field: SerializeField] private SceneConfiguration sceneConfiguration;
    [field: SerializeField] private GameObject nodeParent;
    [field: SerializeField] private GameObject edgeParent;
    [field: SerializeField] private Vector3 offset = new Vector3(-1f, 2f, -3f);
    [field: SerializeField] private DataReader DataReader;
    [field: SerializeField] private float scalingFactor = 1f;

    [Header("Prefabs")]
    [field: SerializeField] private GameObject pre_Node;
    [field: SerializeField] private GameObject pre_Edge;

    [Header("Visual Encoding")]
    [field: SerializeField] private Color groupColor;
    [field: SerializeField] private Color channelColor;
    [field: SerializeField] private float maxEdgeWidth;
    [field: SerializeField] private float minEdgeWidth;
    [field: SerializeField] private Color maxEdgeColor;
    [field: SerializeField] private Color minEdgeColor;
    [field: SerializeField] private float minNodeSize;
    [field: SerializeField] private float maxNodeSize;


    void Start()
    {
        GetLists();
        CreateNodes();
        switch (sceneConfiguration.GetComponent<SceneConfiguration>().AppState)
        {
            case AppState.Network:
             
                LayOutNodes(NodeObjects);
                OffsetNodes(offset);
                CreateEdges();
                ForNodesAndEdgesFillConnectionProperties();
                SetEdgePositionsandWidth();
                SizeNodes();
                break;
            case AppState.Geospatial:
                break;
            default:
                break;
        }

    }

    void CreateEdges()
    {
        foreach (var edge in edges)
        {
            GameObject line = Instantiate(pre_Edge);
            EdgeData data = line.AddComponent<EdgeData>();
            data.Source = edge.SourceID;
            data.Target = edge.TargetID;
            data.Weight = edge.Weight;
            data.TimeStep = edge.TimeStep;

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

            float maxWeight = GetWeights(edges);
            float width = Mathf.Lerp(minEdgeWidth, maxEdgeWidth, data.Weight / maxWeight);
            line.GetComponent<LineRenderer>().startWidth = width;
            line.GetComponent<LineRenderer>().endWidth = width;

            line.GetComponent<LineRenderer>().startColor = Color.Lerp(minEdgeColor, maxEdgeColor, data.Weight / maxWeight);
            line.GetComponent<LineRenderer>().endColor = Color.Lerp(minEdgeColor, maxEdgeColor, data.Weight / maxWeight);

            line.transform.parent = edgeParent.transform;
        }
    }

    void GetLists()
    {
        nodes = DataReader.Nodes;
        edges = DataReader.Edges;
    }

    float GetWeights(List<Edge> collection)
    {
        List<float> weights = new List<float>();
        foreach (var item in collection)
        {
            weights.Add(item.Weight);
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
        edgeParent.transform.Translate(offset);
        nodeParent.transform.Translate(offset);
    }

    void CreateNodes()
    {
        foreach (var node in nodes)
        {
            GameObject mark = Instantiate(pre_Node);
            NodeData data = mark.AddComponent<NodeData>();
            data.Id = node.Id;
            data.EntityType = node.EntityType;
            data.Position = node.Position;
            data.Activities = node.MonthlyActions;

            mark.transform.parent = nodeParent.transform;

            if (node.EntityType == "Group")
            {
                mark.GetComponent<Renderer>().material.color = groupColor;
                groupSymbols.Add(mark);
            }
            else
            {
                mark.GetComponent<Renderer>().material.color = channelColor;
                channelSymbols.Add(mark);
            }

            NodeObjects.Add(mark);
        }

        // Debug.LogFormat("First node has {0} active users in month {1}", NodeObjects[0].GetComponent<NodeData>().Activities.Wrapper[10].ActiveUsers,
        //  NodeObjects[0].GetComponent<NodeData>().Activities.Wrapper[10].SentAt);

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
                Mathf.Lerp(minNodeSize, maxNodeSize, item.GetComponent<NodeData>().IncomingEdges.Count / max),
                Mathf.Lerp(minNodeSize, maxNodeSize, item.GetComponent<NodeData>().IncomingEdges.Count / max),
                Mathf.Lerp(minNodeSize, maxNodeSize, item.GetComponent<NodeData>().IncomingEdges.Count / max)
            );
        }
    }
}
