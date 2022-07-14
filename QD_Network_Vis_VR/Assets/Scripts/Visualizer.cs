using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Visualizer : MonoBehaviour
{
    [Header("Entities")]
    [field: SerializeField] public List<GameObject> EdgeObjects = new List<GameObject>();
    [field: SerializeField] public List<GameObject> NodeObjects = new List<GameObject>();
    [field: SerializeField] public List<GameObject> CityObjects = new List<GameObject>();
    [field: SerializeField] private List<Node> nodes;
    [field: SerializeField] private List<Edge> edges;
    [field: SerializeField] private List<City> cities;
    [field: SerializeField] private List<GameObject> groupSymbols = new List<GameObject>();
    [field: SerializeField] private List<GameObject> channelSymbols = new List<GameObject>();
    [field: SerializeField] Dictionary<float, List<GameObject>> DictLatNode = new Dictionary<float, List<GameObject>>();

    [Header("Scene Setup")]
    [field: SerializeField] private SceneConfiguration sceneConfiguration;
    [field: SerializeField] private AppState currentState;
    [field: SerializeField] private GameObject nodeParent;
    [field: SerializeField] private GameObject edgeParent;
    [field: SerializeField] private GameObject cityParent;
    [field: SerializeField] private Vector3 offsetNetwork = new Vector3(-1f, 2f, -3f);
    [field: SerializeField] private Vector3 offsetGeospatial = new Vector3(-12f, 2f, -50f);
    [field: SerializeField] private DataReader DataReader;
    [field: SerializeField] private float scalingFactor = 1f;

    [Header("Prefabs")]
    [field: SerializeField] private GameObject pre_Node;
    [field: SerializeField] private GameObject pre_Edge;
    [field: SerializeField] private GameObject pre_City;

    [Header("Visual Encoding")]
    [field: SerializeField] private Color groupColor;
    [field: SerializeField] private Color channelColor;
    [field: SerializeField] private float maxEdgeWidth;
    [field: SerializeField] private float minEdgeWidth;
    [field: SerializeField] private Color maxEdgeColor;
    [field: SerializeField] private Color minEdgeColor;
    [field: SerializeField] private float minNodeSize;
    [field: SerializeField] private float maxNodeSize;
    [field: SerializeField] private float verticcalOffsetForNodeStacks = .5f;

    void Start()
    {
        SetAppState();

        GetLists();
        CreateNodes();
        LayOutNodes();
        if (currentState == AppState.Geospatial)
        {
            CrateAndPlaceCities();
            StackNodes();
            OffsetNodes(offsetGeospatial);
        }
        else
        {
            OffsetNodes(offsetNetwork);
        }

        CreateEdges();
        ForNodesAndEdgesFillConnectionProperties();
        SetEdgePositionsandWidth();
        SizeNodes();


    }

    void SetAppState()
    {
        currentState = sceneConfiguration.GetComponent<SceneConfiguration>().AppState;
    }

    void CrateAndPlaceCities()
    {
        for (int i = 0; i < cities.Count; i++)
        {
            City cityRawData = cities[i];
            GameObject city = Instantiate(pre_City);

            CityData data = city.AddComponent<CityData>();
            data.name = cityRawData.Name;
            data.Latitude = cityRawData.Latitude;
            data.Longitude = cityRawData.Longitude;
            data.Population = cityRawData.Population;

            city.transform.position = new Vector3(
                data.Longitude,
                0f,
                data.Latitude
            );
            city.transform.parent = cityParent.transform;
            CityObjects.Add(city);
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
        cities = DataReader.Cities;
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

    void LayOutNodes()
    {
        for (int i = 0; i < NodeObjects.Count; i++)
        {
            if (currentState == AppState.Network)
            {
                NodeObjects[i].transform.position = NodeObjects[i].GetComponent<NodeData>().Position * scalingFactor;
            }
            else
            {
                NodeObjects[i].transform.position = new Vector3(
                NodeObjects[i].GetComponent<NodeData>().Longitude,
                0f,
                NodeObjects[i].GetComponent<NodeData>().Latitude
                );
            }
        }
    }

    void OffsetNodes(Vector3 offset)
    {
        edgeParent.transform.Translate(offset);
        nodeParent.transform.Translate(offset);
        cityParent.transform.Translate(offset);
        cityParent.transform.Translate(0f, -.5f, 0f);
    }

    void StackNodes()
    {
        for (int i = 0; i < NodeObjects.Count; i++)
        {
            NodeData data = NodeObjects[i].GetComponent<NodeData>();

            if (DictLatNode.ContainsKey(data.Latitude))
            {
                DictLatNode[data.Latitude].Add(data.gameObject);
            }
            else
            {
                DictLatNode.Add(data.Latitude, new List<GameObject> { data.gameObject });
            }
        }
        foreach (var kvp in DictLatNode)
        {
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                kvp.Value[i].transform.Translate(0f, verticcalOffsetForNodeStacks * i, 0f);
            }
        }
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
            data.Latitude = node.Latitude;
            data.Longitude = node.Longitude;

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
