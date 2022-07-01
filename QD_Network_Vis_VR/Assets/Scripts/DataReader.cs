using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataReader : MonoBehaviour
{

    [SerializeField] private HashSet<Node> nodesTemp = new HashSet<Node>();
    public List<Node> Nodes { get; set; }
    public List<Edge> Edges { get; set; }

    [SerializeField] private string fileNameEdgeListVr3DCoords = "";
    [SerializeField] private string fileNameEntityActivityTable = "";
    private Dictionary<string, int> outDegreeCounts = new Dictionary<string, int>();
    private Dictionary<string, int> inDegreeCounts = new Dictionary<string, int>();
    [SerializeField] private Dictionary<string, string> nameToEntityTypeMapping = new Dictionary<string, string>();

    void Awake()
    {
        Nodes = new List<Node>();
        Edges = new List<Edge>();
        ReadCSV();
        ConverttoList();
        GetAndAssignEntityType();
        AddDegreesToNodes();
    }

    void ReadCSV()
    {
        using (var reader = new StreamReader("Assets/Data/" + fileNameEdgeListVr3DCoords + ".csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.Split(',')[0] != "from_name")
                {
                    Node newNode = new Node(line.Split(',')[0], new Vector3(float.Parse(line.Split(',')[4]), float.Parse(line.Split(',')[5]), float.Parse(line.Split(',')[6])));
                    nodesTemp.Add(newNode);

                    GetCount(outDegreeCounts, newNode.Id);
                    GetCount(inDegreeCounts, line.Split(',')[1]);

                    Edge newEdge = new Edge(line.Split(',')[0], line.Split(',')[1], float.Parse(line.Split(',')[2]), float.Parse(line.Split(',')[3]));
                    Edges.Add(newEdge);
                }
            }
        }

    }

    void ConverttoList()
    {
        foreach (var item in nodesTemp)
        {
            Nodes.Add(item);
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
        CheckForMissingKeys(outDegreeCounts, inDegreeCounts);
        CheckForMissingKeys(inDegreeCounts, outDegreeCounts);

        for (int i = 0; i < Nodes.Count; i++)
        {
            Node n = Nodes[i];
            n.OutDegree = outDegreeCounts[n.Id];
            n.InDegree = inDegreeCounts[n.Id];
            Nodes[i] = n;
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

    void GetAndAssignEntityType()
    {
        using (var reader = new StreamReader("Assets/Data/" + fileNameEntityActivityTable + ".csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.Split(',')[0] != "HANDLE")
                {
                    string id = line.Split(',')[0];
                    if (!nameToEntityTypeMapping.ContainsKey(id))
                    {
                        nameToEntityTypeMapping.Add(line.Split(',')[0], line.Split(',')[2]);
                    };
                }
            }
        }

        for (int i = 0; i < Nodes.Count; i++)
        {
            Node n = Nodes[i];
            if (nameToEntityTypeMapping.ContainsKey(Nodes[i].Id)) n.EntityType = nameToEntityTypeMapping[Nodes[i].Id];
            Nodes[i] = n;
        }
    }
}


[Serializable]
public struct Node
{
    public string Id { get; set; }
    public string EntityType { get; set; }
    public int InDegree { get; set; }
    public int OutDegree { get; set; }
    public Vector3 Position { get; set; }

    public Node(string id, Vector3 position)
    {
        this.Id = id;
        this.Position = position;
        this.InDegree = 0;
        this.OutDegree = 0;
        this.EntityType = "";
    }

    public override string ToString()
    {
        return "Node with ID: " + this.Id + " and EntityType: " + this.EntityType;
    }
}

[Serializable]
public class Edge
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



