using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityData : MonoBehaviour
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public float Latitude { get; set; }
    [field: SerializeField] public float Longitude { get; set; }
    [field: SerializeField] public int Population { get; set; }
}
