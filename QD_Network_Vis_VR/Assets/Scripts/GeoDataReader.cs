using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GeoDataReader : MonoBehaviour
{
    static public HashSet<Entity> m_Entities = new HashSet<Entity>();
    static public Dictionary<string, float[]> m_GermanCities = new Dictionary<string, float[]>();
    [SerializeField] private string m_FileNameEntities;
    [SerializeField] private string m_FileNameCities;

    void Awake()
    {
        ReadCSV();
        ReadCityCoords();
    }

    void ReadCityCoords()
    {
        using (var reader = new StreamReader("Assets/Data/" + m_FileNameCities))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.Split(',')[0] == "city")
                {
                    continue;
                }

                m_GermanCities.Add(line.Split(',')[0], new float[] { float.Parse(line.Split(',')[1]), float.Parse(line.Split(',')[2]) });
            }
        }
    }

    void ReadCSV()
    {
        using (var reader = new StreamReader("Assets/Data/" + m_FileNameEntities))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string handle = line.Split(',')[0];
                string lat = line.Split(',')[5];

                if (handle == "HANDLE" || lat == "")
                {
                    continue;
                }

                Entity e = new Entity(line.Split(',')[0], line.Split(',')[2], float.Parse(line.Split(',')[5]), float.Parse(line.Split(',')[6]), Random.Range(0, .5f));
                m_Entities.Add(e);

            }
        }
    }

}

public struct Entity
{
    public string name;
    public string entityType;
    public float lat;
    public float lon;

    public float activeUsers;
    public Entity(string name, string entityType, float lat, float lon, float activeUsers)
    {
        this.name = name;
        this.entityType = entityType;
        this.lat = lat;
        this.lon = lon;
        this.activeUsers = activeUsers;
    }
}

public struct City
{
    public string name;
    public float lat;
    public float lon;
}