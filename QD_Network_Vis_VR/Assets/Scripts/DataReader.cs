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
                            n.MonthlyActions.Wrapper.Add(new Activity(line.Split(',')[0], line.Split(',')[1], line.Split(',')[2], Convert.ToInt32(line.Split(',')[3]), Convert.ToInt32(line.Split(',')[4]), line.Split(',')[5], line.Split(',')[6]));
                        }
                        Nodes[i] = n;
                    }
                }
            }
        }
    }
}

[Serializable]
public class Activity
{
    [field: SerializeField] public string SentAt { get; private set; }
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string Group { get; private set; }
    [field: SerializeField] public int PostsTotal { get; private set; }
    [field: SerializeField] public int ActiveUsers { get; private set; }
    [field: SerializeField] public string Latitude { get; private set; }
    [field: SerializeField] public string Longitude { get; private set; }

    public Activity(string id, string sentAt, string group, int postsTotal, int activeUsers, string latitude, string longitude)
    {
        this.Id = id;
        this.SentAt = sentAt;
        this.Group = group;
        this.PostsTotal = postsTotal;
        this.ActiveUsers = activeUsers;
        this.Latitude = latitude;
        this.Longitude = longitude;
    }
}

[Serializable]
public class MonthlyActionWrapper
{
    [SerializeField] public List<Activity> Wrapper;
}

[Serializable]
public struct Node
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string EntityType { get; set; }
    [field: SerializeField] public Vector3 Position { get; private set; }
    [field: SerializeField] public MonthlyActionWrapper MonthlyActions { get; set; }

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
    [field: SerializeField] public string sourceID { get; private set; }
    [field: SerializeField] public string targetID { get; private set; }
    [field: SerializeField] public float weight { get; private set; }
    [field: SerializeField] public float sentAtQuarter { get; private set; }

    public Edge(string sourceID, string targetID, float sentAtQuarter, float weight)
    {
        this.sourceID = sourceID;
        this.targetID = targetID;
        this.sentAtQuarter = sentAtQuarter;
        this.weight = weight;
    }
}



