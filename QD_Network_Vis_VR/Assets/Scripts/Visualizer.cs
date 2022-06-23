using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public GameObject m_ChannelEntities;
    public GameObject m_GroupEntities;

    public GameObject m_EdgeParent;
    public DataReader DataReader;

    public GameObject pre_Node;
    public GameObject pre_Edge;
    public Color m_Blue;
    public Color m_Pink;

    public float m_MaxEdgeWidth;
    public float m_MinEdgeWidth;

    public Color m_MaxColor;
    public Color m_MinColor;
    [SerializeField] private float m_MinSize;
    [SerializeField] private float m_MaxSize;
    [SerializeField] float m_Width;
    private List<GameObject> m_GroupSymbols = new List<GameObject>();
    private List<GameObject> m_ChannelSymbols = new List<GameObject>();
    private List<GameObject> m_EdgeSymbols = new List<GameObject>();
    private List<GameObject> m_AllSymbols = new List<GameObject>();
    private List<Node> m_Nodes;
    private List<Edge> m_Edges;
    void Start()
    {
        m_Nodes = DataReader.m_Nodes;
        m_Edges = DataReader.m_Edges;
        CreateNodes();
        SizeNodes();
        LayOutNodes(m_GroupSymbols, m_GroupEntities.transform.position);
        LayOutNodes(m_ChannelSymbols, m_ChannelEntities.transform.position);
        DrawEdges(m_Edges);
    }

    void DrawEdges(List<Edge> collection)
    {
        float maxWeight = GetWeights(m_Edges);
        foreach (var item in collection)
        {
            GameObject line = Instantiate(pre_Edge);
            line.GetComponent<LineRenderer>().SetPositions(
                new Vector3[2]{
                IdentifyNode(item.sourceID, m_AllSymbols).transform.position,
                IdentifyNode(item.targetID, m_AllSymbols).transform.position,
        }
            );
            float width = Mathf.Lerp(m_MinEdgeWidth, m_MaxEdgeWidth, item.weight / maxWeight);
            line.GetComponent<LineRenderer>().startWidth = width;
            line.GetComponent<LineRenderer>().endWidth = width;

            line.GetComponent<LineRenderer>().startColor = Color.Lerp(m_MinColor, m_MaxColor, item.weight / maxWeight);
            line.GetComponent<LineRenderer>().endColor = Color.Lerp(m_MinColor, m_MaxColor, item.weight / maxWeight);
            m_EdgeSymbols.Add(line);
            line.transform.parent = m_EdgeParent.transform;
            // line.GetComponent<LineRenderer>().material.color = Color.Lerp(m_MinColor, m_MaxColor, item.weight / maxWeight);
        }
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
            if (item.GetComponent<Record>().m_Id == id)
            {
                return item;
            }
        }
        return new GameObject();
    }

    void LayOutNodes(List<GameObject> symbols, Vector3 center)
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            Vector3 newPos = new Vector3(center.x + Random.Range(-m_Width / 2, m_Width / 2), center.y, center.z + Random.Range(-m_Width/2, m_Width/2));
            symbols[i].transform.position = newPos;
        }
    }

    void CreateNodes()
    {
        Debug.Log("m_Nodes.Count: " + m_Nodes.Count);
        foreach (var node in m_Nodes)
        {
            GameObject mark = Instantiate(pre_Node);
            mark.AddComponent<Record>();
            mark.GetComponent<Record>().m_Id = node.id;
            mark.GetComponent<Record>().m_EntityType = node.entityType;
            mark.GetComponent<Record>().m_Degree = node.m_Degree;
            mark.GetComponent<Record>().m_OutDegree = node.m_OutDegree;
            mark.GetComponent<Record>().m_InDegree = node.m_InDegree;

            if (node.entityType == "Group")
            {
                mark.GetComponent<Renderer>().material.color = m_Blue;
                mark.transform.position = m_GroupEntities.transform.position;
                mark.transform.parent = m_GroupEntities.transform;
                m_GroupSymbols.Add(mark);
            }
            else
            {
                mark.GetComponent<Renderer>().material.color = m_Pink;
                mark.transform.position = m_ChannelEntities.transform.position;
                mark.transform.parent = m_ChannelEntities.transform;
                m_ChannelSymbols.Add(mark);
            }
        }

        foreach (var item in m_GroupSymbols)
        {
            m_AllSymbols.Add(item);
        }
        foreach (var item in m_ChannelSymbols)
        {
            m_AllSymbols.Add(item);
        }
    }

    void SizeNodes()
    {
        List<float> degrees = new List<float>();
        foreach (var item in m_Nodes)
        {
            degrees.Add(item.m_Degree);
        }
        float max = Mathf.Max(degrees.ToArray());

        foreach (var item in m_AllSymbols)
        {
            item.gameObject.transform.localScale = new Vector3(
                Mathf.Lerp(m_MinSize, m_MaxSize, item.GetComponent<Record>().m_Degree / max),
                Mathf.Lerp(m_MinSize, m_MaxSize, item.GetComponent<Record>().m_Degree / max),
                Mathf.Lerp(m_MinSize, m_MaxSize, item.GetComponent<Record>().m_Degree / max)
            );
        }
    }
}
