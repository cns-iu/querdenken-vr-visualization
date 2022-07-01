using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record : MonoBehaviour
{
    public string Id;
    public string EntityType;
    public int InDegree;
    public int OutDegree;
    public float ActiveUsers;
    public Vector3 Position;
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
