using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;


public class OsmClient : MonoBehaviour
{

    OsmData data;

    void Start()
    {

            data = parseOsmData();
        
        
    }
    void Update(){
        Debug.Log(data.getRoadsCount());
    }

   
   public OsmData parseOsmData(){
        OsmData osmData = new OsmData();
        XmlDocument xmlDoc = new XmlDocument();
        string filePath = Application.dataPath + "/Osm/map.xml";

        xmlDoc.Load(filePath);

        //xmlDoc.LoadXml(osmFile.text);

        XmlNodeList nodeElements = xmlDoc.GetElementsByTagName("node");
        XmlNodeList wayElements = xmlDoc.GetElementsByTagName("way");

        foreach(XmlNode node in nodeElements){
            Debug.Log(node.Attributes["id"].Value);
            Debug.Log(node.Attributes["lat"].Value);
            Debug.Log(node.Attributes["lon"].Value);
            string id = node.Attributes["id"].Value;
            double lat = XmlConvert.ToDouble(node.Attributes["lat"].Value);
            double lon = XmlConvert.ToDouble(node.Attributes["lon"].Value);
            OsmNode osmNode = new OsmNode(lat, lon);
            XmlNodeList tagElements = node.SelectNodes("tag");
             foreach (XmlNode tag in tagElements){
                string key = tag.Attributes["k"].Value;
                string value = tag.Attributes["v"].Value;
                //todo
            }

            //add to osm nodes
        }

        foreach (XmlNode way in wayElements){
            string id = way.Attributes["id"].Value;
            XmlNodeList tagElements = way.SelectNodes("tag");
            foreach (XmlNode tag in tagElements){
                string key = tag.Attributes["k"].Value;
                string value = tag.Attributes["v"].Value;
                //todo
            }

            XmlNodeList ndElements = way.SelectNodes("nd");
            List<OsmNode> roadPoints = new List<OsmNode>();
            foreach (XmlNode nd in ndElements)
            {
                string refId = nd.Attributes["ref"].Value;
                // todo
            }

            OsmRoad road = new OsmRoad(id, roadPoints);
            osmData.AddRoad(road);
        }

        return osmData;
   }
 
}
