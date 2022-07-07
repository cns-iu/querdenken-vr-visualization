using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataReader : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private HashSet<Node> nodesTemp = new HashSet<Node>();
    public List<Node> Nodes { get; set; }
    public List<Edge> Edges { get; set; }

    [Header("Files")]
    [SerializeField] private string fileNameEdgeListVr3DCoords = "";
    [SerializeField] private string fileNameEntityActivityTable = "";

    [Header("Counts")]
    [SerializeField] private Dictionary<string, string> nameToEntityTypeMapping = new Dictionary<string, string>();

    void Awake()
    {
        Nodes = new List<Node>();
        Edges = new List<Edge>();
        ReadCSV();
        ConverttoList();
        ForNodesGetEntityType();
        ForNodesGetMonthlyActions();
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

    void ForNodesGetEntityType()
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

    void ForNodesGetMonthlyActions()
    {
        using (var reader = new StreamReader("Assets/Data/" + fileNameEntityActivityTable + ".csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.Split(',')[0] != "HANDLE")
                {
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        Node n = Nodes[i];
                        if (n.MonthlyActions == null)
                        {
                            n.MonthlyActions = new MonthlyActionWrapper();
                            n.MonthlyActions.Wrapper = new List<Activity>();
                        }

                        if (line.Split(',')[0] == n.Id)
                        {
                            n.MonthlyActions.Wrapper.Add(new Activity(line.Split(',')[1], Convert.ToInt32(line.Split(',')[3]), Convert.ToInt32(line.Split(',')[4]), line.Split(',')[5], line.Split(',')[6]));
                        }
                        Nodes[i] = n;
                    }
                }
            }
        }
    }
}

[Serializable]
public struct Activity
{
    public string SentAt { get; set; }
    public int PostsTotal { get; set; }
    public int ActiveUsers { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }

    public Activity(string sentAt, int postsTotal, int activeUsers, string latitude, string longitude)
    {
        this.SentAt = sentAt;
        this.PostsTotal = postsTotal;
        this.ActiveUsers = activeUsers;
        this.Latitude = latitude;
        this.Longitude = longitude;
    }
}

[Serializable]
public class MonthlyActionWrapper
{
    public List<Activity> Wrapper;
}

[Serializable]
public struct Node
{
    public string Id { get; set; }
    public string EntityType { get; set; }
    public Vector3 Position { get; set; }
    public MonthlyActionWrapper MonthlyActions;

    public Node(string id, Vector3 position)
    {
        this.Id = id;
        this.Position = position;
        this.EntityType = "";
        this.MonthlyActions = null;
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



