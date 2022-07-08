using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConfiguration : MonoBehaviour
{
    [field: SerializeField] public int StartTimeStep { get; set; }
    [field: SerializeField] public int EndTimeStep { get; set; }

    [SerializeField] private Visualizer visualizer;
    [SerializeField] private List<GameObject> edgeObjects;

    private void Start()
    {
        edgeObjects = visualizer.EdgeObjects;
    }

    public void FilterByTimeStep()
    {
        for (int i = 0; i < edgeObjects.Count; i++)
        {
            EdgeData e = edgeObjects[i].GetComponent<EdgeData>();
            e.gameObject.SetActive(e.TimeStep >= StartTimeStep && e.TimeStep <= EndTimeStep);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FilterByTimeStep();
        }
    }
}
