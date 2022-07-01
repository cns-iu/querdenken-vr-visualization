using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject m_VisParent;
    [SerializeField] private float m_DisplayHeight;
    [SerializeField] private float m_RescaleFactor;
    [SerializeField] private Color m_Blue;
    [SerializeField] private Color m_Pink;
    [SerializeField] private HashSet<Entity> m_Entities = new HashSet<Entity>();
    [SerializeField] private List<GameObject> m_Marks = new List<GameObject>();
    [SerializeField] private GameObject pre_Node;
    [SerializeField] private GameObject pre_Bar;

    void Start()
    {
        CreateNodes();
        LayOutNodes();
        // CreateBars();
    }

    void CreateNodes()
    {
        m_Entities = GeoDataReader.m_Entities;
        GameObject g = new GameObject();

        foreach (Entity e in m_Entities)
        {
            g = Instantiate(pre_Node);
            g.AddComponent<Record>().EntityType = e.entityType;
            g.GetComponent<Record>().Id = e.name;
            g.GetComponent<Record>().X = e.lat;
            g.GetComponent<Record>().Z = e.lon;
            g.GetComponent<Record>().ActiveUsers = e.activeUsers;

            if (e.entityType == "Group")
            {
                g.GetComponent<Renderer>().material.color = m_Blue;
            }
            else
            {
                g.GetComponent<Renderer>().material.color = m_Pink;
            }
            g.transform.parent = m_VisParent.transform;
            m_Marks.Add(g);
        }
    }

    void LayOutNodes()
    {
        for (int i = 0; i < m_Marks.Count; i++)
        {
            m_Marks[i].transform.position = new Vector3(RescaleCoord(m_Marks[i].GetComponent<Record>().X), m_DisplayHeight, RescaleCoord(m_Marks[i].GetComponent<Record>().Z) * 1.3f);
        }

        foreach (var city in GeoDataReader.m_GermanCities)
        {
            GameObject g = Instantiate(pre_Bar);
            g.transform.position = new Vector3(RescaleCoord(city.Value[1]), m_DisplayHeight, RescaleCoord(city.Value[0]) * 1.3f);
            g.transform.parent = m_VisParent.transform;
        }
    }

    void CreateBars()
    {
        foreach (var item in m_Marks)
        {
            Debug.Log("running");
            GameObject b = Instantiate(pre_Bar);
            b.transform.position = new Vector3(
                item.transform.position.x,
            b.transform.position.y,
            item.transform.position.z
            );
            b.transform.localScale = new Vector3(
                b.transform.localScale.x,
                 item.GetComponent<Record>().ActiveUsers,
                 b.transform.localScale.z
                 );
        }
    }

    float RescaleCoord(float val)
    {
        float rescaled = val / m_RescaleFactor;

        return rescaled;
    }

}
