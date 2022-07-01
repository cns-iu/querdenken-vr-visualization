using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [Header("Entities")]
    [SerializeField] private List<GameObject> GroupSymbols = new List<GameObject>();
    [SerializeField] private List<GameObject> ChannelSymbols = new List<GameObject>();
    [SerializeField] private List<GameObject> EdgeSymbols = new List<GameObject>();
    [SerializeField] private List<GameObject> AllSymbols = new List<GameObject>();
    [SerializeField] private List<Node> Nodes;
    [SerializeField] private List<Edge> Edges;

    [Header("Scene Setup")]
    [SerializeField] private GameObject NodeParent;
    [SerializeField] private GameObject EdgeParent;
    [SerializeField] private Vector3 offset = new Vector3(-1f, 2f, -3f);
    [SerializeField] private DataReader DataReader;
    [SerializeField] private float scalingFactor = 1f;

    [Header("refabs")]
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
        SizeNodes();
        LayOutNodes(AllSymbols);
        OffsetNodes(offset);
        DrawEdges(Edges);
    }

    void DrawEdges(List<Edge> collection)
    {
        float maxWeight = GetWeights(Edges);
        foreach (var item in collection)
        {
            GameObject line = Instantiate(pre_Edge);
            line.GetComponent<LineRenderer>().SetPositions(
                new Vector3[2]{
                IdentifyNode(item.sourceID, AllSymbols).transform.position,
                IdentifyNode(item.targetID, AllSymbols).transform.position,
        }
            );
            float width = Mathf.Lerp(MinEdgeWidth, MaxEdgeWidth, item.weight / maxWeight);
            line.GetComponent<LineRenderer>().startWidth = width;
            line.GetComponent<LineRenderer>().endWidth = width;

            line.GetComponent<LineRenderer>().startColor = Color.Lerp(MinColor, MaxColor, item.weight / maxWeight);
            line.GetComponent<LineRenderer>().endColor = Color.Lerp(MinColor, MaxColor, item.weight / maxWeight);
            EdgeSymbols.Add(line);
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

    GameObject IdentifyNode(string id, List<GameObject> list)
    {
        foreach (var item in list)
        {
            if (item.GetComponent<Record>().Id == id)
            {
                return item;
            }
        }
        return new GameObject();
    }

    void LayOutNodes(List<GameObject> symbols)
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            symbols[i].transform.position = symbols[i].GetComponent<Record>().Position * scalingFactor;
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
            mark.AddComponent<Record>();
            mark.GetComponent<Record>().Id = node.Id;
            mark.GetComponent<Record>().EntityType = node.EntityType;
            mark.GetComponent<Record>().OutDegree = node.OutDegree;
            mark.GetComponent<Record>().InDegree = node.InDegree;
            mark.GetComponent<Record>().Position = node.Position;

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
        }

        foreach (var item in GroupSymbols)
        {
            AllSymbols.Add(item);
        }
        foreach (var item in ChannelSymbols)
        {
            AllSymbols.Add(item);
        }
    }

    void SizeNodes()
    {
        List<float> degrees = new List<float>();
        foreach (var item in Nodes)
        {
            degrees.Add(item.InDegree + item.OutDegree);
        }
        float max = Mathf.Max(degrees.ToArray());

        foreach (var item in AllSymbols)
        {
            item.gameObject.transform.localScale = new Vector3(
                Mathf.Lerp(MinSize, MaxSize, item.GetComponent<Record>().InDegree / max),
                Mathf.Lerp(MinSize, MaxSize, item.GetComponent<Record>().InDegree / max),
                Mathf.Lerp(MinSize, MaxSize, item.GetComponent<Record>().InDegree / max)
            );
        }
    }
}
