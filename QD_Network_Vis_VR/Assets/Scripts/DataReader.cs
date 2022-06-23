using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataReader : MonoBehaviour
{

    public HashSet<Node> m_NodesTemp = new HashSet<Node>();
    public List<Node> m_Nodes = new List<Node>();

    //[0] in-m_Degree, [1]  out-m_Degree
    public Dictionary<string, int> m_OutDegreeCounts = new Dictionary<string, int>();
    public Dictionary<string, int> m_InDegreeCounts = new Dictionary<string, int>();
    public List<Edge> m_Edges = new List<Edge>();
    [SerializeField] private string m_Filename = "prelim_network_data_QD.csv";
    // Start is called before the first frame update
    void Awake()
    {
        ReadCSV();
        ConvertToList();
        AddDegreesToNodes();
    }
    void ReadCSV()
    {
        using (var reader = new StreamReader("Assets/Data/" + m_Filename))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.Split(',')[0] != "from_name")
                {
                    Node newNode = new Node(line.Split(',')[0], "", new Vector3(float.Parse(line.Split(',')[4]),float.Parse(line.Split(',')[5]),float.Parse(line.Split(',')[6])));
                    m_NodesTemp.Add(newNode);

                    GetCount(m_OutDegreeCounts, newNode.id);
                    GetCount(m_InDegreeCounts, line.Split(',')[1]);

                    Edge newEdge = new Edge(line.Split(',')[0], line.Split(',')[1], float.Parse(line.Split(',')[2]), float.Parse(line.Split(',')[3]));
                    m_Edges.Add(newEdge);
                }
            }
        }
    }
    void GetCount(Dictionary<string, int> dict, string id)
    {
        if (dict.ContainsKey(id))
        {
            dict[id]++;
        }
        else
        {
            dict.Add(id, 1);
        }
    }

    void AddDegreesToNodes()
    {
        CheckForMissingKeys(m_OutDegreeCounts, m_InDegreeCounts);
        CheckForMissingKeys(m_InDegreeCounts, m_OutDegreeCounts);

        for (int i = 0; i < m_Nodes.Count; i++)
        {
            Node n = m_Nodes[i];
            n.m_OutDegree = m_OutDegreeCounts[n.id];
            n.m_InDegree = m_InDegreeCounts[n.id];
            n.m_Degree = n.m_InDegree + n.m_OutDegree;
            m_Nodes[i] = n;
        }
    }

    void CheckForMissingKeys(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
    {
        foreach (var item in dict1)
        {
            if (!dict2.ContainsKey(item.Key))
            {
                dict2.Add(item.Key, 0);
            }
        }
    }

    void ConvertToList()
    {
        foreach (var item in m_NodesTemp)
        {
            m_Nodes.Add(item);
        }
    }
}



public struct Node
{
    public string id;
    public string entityType;
    public int m_InDegree;
    public int m_OutDegree;
    public int m_Degree;

    public Vector3 m_Position;
    public Node(string id, string entityType, Vector3 position)
    {
        this.id = id;
        this.entityType = entityType;
        this.m_Position = position;
        this.m_InDegree = 0;
        this.m_OutDegree = 0;
        this.m_Degree = 0;
    }

    public void ComputeDegree()
    {

    }

    public override string ToString()
    {
        return "Node with ID: " + this.id + " and EntityType: " + this.entityType;
    }
}

public struct Edge
{
    public string sourceID;
    public string targetID;
    public float weight;
    public float sentAtQuarter;

    public Edge(string sourceID, string targetID, float sentAtQuarter, float weight)
    {
        this.sourceID = sourceID;
        this.targetID = targetID;
        this.sentAtQuarter = sentAtQuarter;
        this.weight = weight;
    }
}




