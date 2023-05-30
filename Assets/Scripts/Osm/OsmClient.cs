using System;
using System.Xml;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct OsmData{
    public Dictionary<string, OsmNode> nodesDictionary;
    public Dictionary<string, List<string>> waysDictionary;
}

public struct OsmNode{
    public Vector3 point;
}


public class OsmClient : MonoBehaviour
{
    
    void Start(){

        //OsmData test = readOsmFile(Application.dataPath + "/Osm/map.xml");
    }

    public OsmData readOsmFile(string filePath){
        OsmData osmData;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filePath);
        osmData.nodesDictionary = loadOsmNodes(xmlDoc);
        osmData.waysDictionary = loadOsmWays(xmlDoc);
        return osmData;
    }
    
    // OSM Nodes
    private Dictionary<string, OsmNode> loadOsmNodes(XmlDocument xmlDocument){
        DateTime Start = DateTime.Now;
        
        Dictionary<string, OsmNode> resultDictionary = new Dictionary<string, OsmNode>();
        XmlNodeList nodes = xmlDocument.GetElementsByTagName("node");
        foreach(XmlNode node in nodes){
            OsmNode osmNode = parseToOsmNode(node);
            resultDictionary.Add(node.Attributes["id"].Value, osmNode);
        }

        Debug.Log("OSM nodes loading process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
        return resultDictionary;
    }

    private OsmNode parseToOsmNode(XmlNode node){
        float x = float.Parse(node.Attributes["lat"].Value, CultureInfo.InvariantCulture);
        float z = float.Parse(node.Attributes["lon"].Value, CultureInfo.InvariantCulture);
        OsmNode osmNode;
        osmNode.point = new Vector3(x, 0f, z) ; //* scale;
        return osmNode;
    }

    // OSM Ways
    private Dictionary<string, List<string>> loadOsmWays(XmlDocument xmlDocument){
        DateTime Start = DateTime.Now;

        Dictionary<string, List<string>> resultDictionary = new Dictionary<string, List<string>>();
        XmlNodeList ways = xmlDocument.GetElementsByTagName("way");
        foreach(XmlNode way in ways){
            XmlNodeList nodes = way.SelectNodes("nd");
            List<string> osmNodesRef = new List<string>(); //reference to nodes by id
            foreach(XmlNode node in nodes){
                osmNodesRef.Add(node.Attributes["ref"].Value);
            }
            resultDictionary.Add(way.Attributes["id"].Value, osmNodesRef);
        }

        Debug.Log("OSM ways loading process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
        return resultDictionary;
    }



 
}
