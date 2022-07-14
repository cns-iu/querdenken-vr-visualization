using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AppState { Network, Geospatial }

public class SceneConfiguration : MonoBehaviour
{
    [field: SerializeField] public AppState AppState;
    [field: SerializeField] public int StartTimeStep { get; set; }
    [field: SerializeField] public int EndTimeStep { get; set; }

    public static event Action<SceneConfigurationChange> SceneConfigChange;

    [Header("References in scene")]
    [SerializeField] private Visualizer visualizer;
    [SerializeField] private List<GameObject> edgeObjects;

    private void Start()
    {
        edgeObjects = visualizer.EdgeObjects;
    }

    /// <summary>
    /// Temporary method to add a time filter
    /// </summary>
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

public class SceneConfigurationChange
{
    [field: SerializeField] public AppState NewState { get; set; }
    [field: SerializeField] public int StartTimeStep { get; set; }
    [field: SerializeField] public int EndTimeStep { get; set; }
}
