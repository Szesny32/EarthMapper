using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldBuilder : MonoBehaviour
{
    [SerializeField]
    private OsmClient osmClient;

    public float scale = 10f;
    public float lineWidth = 0.001f; 

    GameObject highways; 
    GameObject buildings; 
    GameObject railways; 
    GameObject naturals; 
    GameObject others; 

    void Start()
    {
        highways = new GameObject("Highways");
        highways.transform.parent = this.transform.parent;

        buildings = new GameObject("Buildings");
        buildings.transform.parent = this.transform.parent;

        railways = new GameObject("Railways");
        railways.transform.parent = this.transform.parent;

        naturals = new GameObject("Naturals");
        naturals.transform.parent = this.transform.parent;

        others = new GameObject("Others");
        others.transform.parent = this.transform.parent; 

        OsmData chunkData = osmClient.readOsmFile(Application.dataPath + "/Osm/map.xml");
        generateChunk(chunkData);
    }

    
    void Update(){
        
    }


    void generateChunk(OsmData chunkData){
        DateTime Start = DateTime.Now;

        foreach (KeyValuePair<string, OsmWay> way in chunkData.waysDictionary){
            List<Vector3> points = new List<Vector3>();
            foreach(string nodeId in way.Value.osmNodes){
                if(chunkData.nodesDictionary.ContainsKey(nodeId))
                    points.Add(chunkData.nodesDictionary[nodeId].point * scale);
            }

            switch(way.Value.category){
                case "highway": 
                    buildRoad(way.Key, points);
                    break;
                case "building": 
                    buildBuilding(way.Key, points);
                    break;
                case "railway": 
                    buildRailways(way.Key, points);
                    break;
                case "natural": 
                    buildNaturals(way.Key, points, way.Value.type);
                    break;
                default:
                    buildOthers(way.Key, points);
                    break;
            }

        }

        Debug.Log("Chunk generation process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
    }
 

    void buildRoad(string id, List<Vector3> points){
            GameObject road = new GameObject("highway_" + id);

            LineRenderer lineRenderer = road.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            road.transform.parent = highways.transform;
    }

    void buildBuilding(string id, List<Vector3> points){
            GameObject building = new GameObject("building_" + id);

            LineRenderer lineRenderer = building.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            lineRenderer.startColor = new Color(0.5f, 1f, 0f, 1f);
            lineRenderer.endColor = new Color(0.5f, 1f, 0f, 1f);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            building.transform.parent = buildings.transform;
    }

    void buildRailways(string id, List<Vector3> points){
            GameObject railway = new GameObject("railway_" + id);

            LineRenderer lineRenderer = railway.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            railway.transform.parent = railways.transform;
    }

    void buildNaturals(string id, List<Vector3> points, string type){
             if(type == "water"){
                GameObject water = new GameObject("natural_" + id);

                Mesh mesh = new Mesh();
                Vector3[] vertices = points.ToArray();
                mesh.vertices = vertices;
                int[] triangles = new int[(points.Count - 2) * 3];
                int triangleIndex = 0;
                for (int i = 1; i < points.Count - 1; i++){
                    triangles[triangleIndex] = 0;
                    triangles[triangleIndex + 1] = i;
                    triangles[triangleIndex + 2] = i + 1;
                    triangleIndex += 3;
                }
                
                mesh.triangles = triangles;
                mesh.RecalculateNormals();

                MeshFilter meshFilter = water.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                
                MeshRenderer meshRenderer = water.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.blue;
                
                water.transform.parent = naturals.transform;
            } else {
                buildOthers(id, points);
            }

       

           
    }

    void buildOthers(string id, List<Vector3> points){
            GameObject other = new GameObject("entity_" + id);

            LineRenderer lineRenderer = other.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            other.transform.parent = others.transform;
    }




}
