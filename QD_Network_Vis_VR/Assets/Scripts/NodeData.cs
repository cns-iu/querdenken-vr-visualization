using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{
    public string Id;
    public string EntityType;
    public float ActiveUsers;
    public Vector3 Position;
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public List<GameObject> OutgoingEdges = new List<GameObject>();
    public List<GameObject> IncomingEdges = new List<GameObject>();
}
