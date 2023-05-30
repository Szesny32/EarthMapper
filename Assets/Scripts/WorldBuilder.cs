using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldBuilder : MonoBehaviour
{
    [SerializeField]
    private  OsmClient osmClient;

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

        foreach (KeyValuePair<string, List<string>> way in chunkData.waysDictionary){
            List<Vector3> points = new List<Vector3>();
            foreach(string nodeId in way.Value){
                if(chunkData.nodesDictionary.ContainsKey(nodeId))
                    points.Add(chunkData.nodesDictionary[nodeId].point * scale);
            }
            GameObject roadObject = new GameObject("Way_" + way.Key);
            LineRenderer lineRenderer = roadObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            roadObject.transform.parent = this.transform.parent;
        }

        Debug.Log("Chunk generation process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
    }

}
