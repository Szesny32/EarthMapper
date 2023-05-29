using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Globalization;
using System;

public class OsmNode{
    public Vector3 point;
    public OsmNode(string lat, string lon){

        float x = float.Parse(lat, CultureInfo.InvariantCulture);
        float z = float.Parse(lon, CultureInfo.InvariantCulture);
        point = new Vector3(x * 10f, 0f, z * 10f);
    }
};


public class OsmData : MonoBehaviour
{
    Dictionary<string, OsmNode> nodesDict;
    Dictionary<string, List<string>> waysDict;

    void Start(){
       

        nodesDict = new Dictionary<string, OsmNode>();
        waysDict = new Dictionary<string, List<string>>();

        DateTime Start = DateTime.Now;
        loadOsmData();
        Debug.Log(" 1/2 Elapsed time = " + DateTime.Now.Subtract(Start).ToString());
        Start = DateTime.Now;
        generateOsmData();
        Debug.Log("2/2 Elapsed time = " + DateTime.Now.Subtract(Start).ToString());


    }


    void loadOsmData(){
        
        XmlNodeList wayNodes;
        string xmlFilePath = Application.dataPath + "/Osm/map2.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);

        XmlNodeList nodes = xmlDoc.GetElementsByTagName("node");
        foreach(XmlNode node in nodes){
            OsmNode osmNode = new OsmNode(node.Attributes["lat"].Value, node.Attributes["lon"].Value);
            nodesDict.Add(node.Attributes["id"].Value, osmNode);
        }

        XmlNodeList ways = xmlDoc.GetElementsByTagName("way");
        foreach(XmlNode way in ways){
            XmlNodeList ndNodes = way.SelectNodes("nd");
            List<string> osmNodes = new List<string>();
            foreach(XmlNode ndNode in ndNodes){
                osmNodes.Add(ndNode.Attributes["ref"].Value);
            }
            waysDict.Add(way.Attributes["id"].Value, osmNodes);
        }
    }


      void generateOsmData(){
        foreach (KeyValuePair<string, List<string>> way in waysDict){


            List<Vector3> points = new List<Vector3>();
            foreach(string nodeId in way.Value){
                if(nodesDict.ContainsKey(nodeId)){  
                    points.Add(nodesDict[nodeId].point);
                }   
            }

            Color lineColor = Color.white;
            GameObject roadObject = new GameObject("Road_" + way.Key);
            LineRenderer lineRenderer = roadObject.AddComponent<LineRenderer>();

            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = 0.001f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            
            lineRenderer.SetPositions(points.ToArray());
            roadObject.transform.parent = this.transform.parent;
        }
    }
}

