using System;
using System.Xml;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct OsmData{
    public OsmBounds osmBounds;
    public Dictionary<string, OsmNode> nodesDictionary;
    public Dictionary<string, OsmWay> waysDictionary;
}

public struct OsmNode{
    public Vector3 point;
}

public struct OsmWay{
    public List<string> osmNodes;
    public string category;
    public string type;
}

public struct OsmBounds{
    public float minLatitude;
    public float maxLatitude;
    public float minLongitude;
    public float maxLongitude;
}


public class OsmClient : MonoBehaviour
{
    private HashSet<string> mainCategories;

    void Start(){
        setMainCategories();
       
        //OsmData test = readOsmFile(Application.dataPath + "/Osm/map.xml");
    }

    // Configuration
    private void setMainCategories(){
        mainCategories = new HashSet<string>();
        mainCategories.Add("highway");
        mainCategories.Add("railway");
        mainCategories.Add("building");
        mainCategories.Add("natural");
        mainCategories.Add("landuse");
        mainCategories.Add("leisure");
        mainCategories.Add("amenity");
    }

    private bool isMainCategory(string category){
        return mainCategories.Contains(category);
    }



    public OsmData readOsmFile(string filePath){
        OsmData osmData;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filePath);
        //test(xmlDoc);
        osmData.osmBounds = getBounds(xmlDoc);
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
    private Dictionary<string, OsmWay> loadOsmWays(XmlDocument xmlDocument){
        DateTime Start = DateTime.Now;

        Dictionary<string, OsmWay> resultDictionary = new Dictionary<string, OsmWay>();
        XmlNodeList ways = xmlDocument.GetElementsByTagName("way");
        foreach(XmlNode way in ways){
            XmlNodeList nodes = way.SelectNodes("nd");
            OsmWay osmWay;
            osmWay.osmNodes = new List<string>(); //reference to nodes by id
            foreach(XmlNode node in nodes){
                osmWay.osmNodes.Add(node.Attributes["ref"].Value);
            }
            KeyValuePair<string, string> mainCategory = getWayMainCategory(way);
            osmWay.category = mainCategory.Key;
            osmWay.type = mainCategory.Value;
            resultDictionary.Add(way.Attributes["id"].Value, osmWay);
        }

        Debug.Log("OSM ways loading process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
        return resultDictionary;
    }

    KeyValuePair<string, string> getWayMainCategory(XmlNode way){
        XmlNodeList tags = way.SelectNodes("tag");
        foreach(XmlNode tag in tags){
            if(isMainCategory(tag.Attributes["k"].Value))
                return new KeyValuePair<string, string>(tag.Attributes["k"].Value, tag.Attributes["v"].Value); 
        }
        return new KeyValuePair<string, string>("", "");

    }

    // OSM bounds
    public OsmBounds getBounds(XmlDocument xmlDoc){
        OsmBounds bounds;
        XmlNodeList nodeList = xmlDoc.GetElementsByTagName("bounds");
        if(nodeList.Count > 0){
            bounds.minLatitude = float.Parse(nodeList[0].Attributes["minlat"].Value, CultureInfo.InvariantCulture);
            bounds.maxLatitude = float.Parse(nodeList[0].Attributes["maxlat"].Value, CultureInfo.InvariantCulture);
            bounds.minLongitude = float.Parse(nodeList[0].Attributes["minlon"].Value, CultureInfo.InvariantCulture);
            bounds.maxLongitude = float.Parse(nodeList[0].Attributes["maxlon"].Value, CultureInfo.InvariantCulture);
        } else {
            Debug.Log("Error: There is no bounds in file");
            bounds.minLatitude = 0f;
            bounds.maxLatitude = 0f;
            bounds.minLongitude = 0f;
            bounds.maxLongitude = 0f;
        }
    
        return bounds;
    }


    // test
    void test(XmlDocument xmlDoc){

        XmlNodeList tagNodes = xmlDoc.GetElementsByTagName("tag");
        HashSet<string> uniqueTags = new HashSet<string>();

        foreach (XmlNode tagNode in tagNodes)
        {
            XmlAttribute kAttribute = tagNode.Attributes["k"];
            if (kAttribute != null)
            {
                string kValue = kAttribute.Value;
                uniqueTags.Add(kValue);
            }
        }

        foreach (string tag in uniqueTags)
        {
            Debug.Log(tag);
        }
    }



 
}
