using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{
    public string Id;
    public string EntityType;
    public Vector3 Position;
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public List<GameObject> OutgoingEdges = new List<GameObject>();
    public List<GameObject> IncomingEdges = new List<GameObject>();
    public MonthlyActionWrapper Activities = new MonthlyActionWrapper();
    public float Latitude;
    public float Longitude;
}
