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
    
    void Start()
    {
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
            GameObject roadObject = new GameObject("Way_" + way.Key);
            LineRenderer lineRenderer = roadObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            Color entityColor = getEntityColor(way.Value.category);
            lineRenderer.startColor = entityColor;
            lineRenderer.endColor = entityColor;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            roadObject.transform.parent = this.transform.parent;
        }

        Debug.Log("Chunk generation process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
    }

    private Color getEntityColor(string category){
        switch(category){
            case "highway": 
                return Color.green;
            default:
                return Color.red;
        }
    }

}
