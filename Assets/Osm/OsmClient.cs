using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Globalization;

public class OsmClient : MonoBehaviour
{



    XmlNodeList wayNodes;


    void Start()
    {

        loadWayNodes();
        //buildRoads();


        
    }
    void Update(){
        
        //Debug.Log(data.getRoadsCount());
    }
 
    void loadWayNodes(){
        string xmlFilePath = Application.dataPath + "/Osm/map2.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);
        wayNodes = xmlDoc.GetElementsByTagName("way");

        // Zainicjalizuj struktury danych
        List<string> roadIds = new List<string>();
        List<string> visibilities = new List<string>();
        List<List<Vector3>> roadPointsList = new List<List<Vector3>>();

        foreach (XmlNode wayNode in wayNodes)
        {
            XmlNodeList tagNodes = wayNode.SelectNodes("tag[@k='highway']");
            if (tagNodes.Count == 0)
                continue;

            // Pobierz atrybuty "id" i "visible" dla drogi
            string roadId = wayNode.Attributes["id"].Value;
            string visible = wayNode.Attributes["visible"].Value;

            // Pobierz wierzchołki (node) dla drogi
            XmlNodeList ndNodes = wayNode.SelectNodes("nd");

            // Zapisz wierzchołki w liście
            List<Vector3> roadPoints = new List<Vector3>();
            foreach (XmlNode ndNode in ndNodes)
            {
                string nodeId = ndNode.Attributes["ref"].Value;
                XmlNode node = xmlDoc.SelectSingleNode("osm/node[@id='" + nodeId + "']");


                float lat = float.Parse(node.Attributes["lat"].Value, CultureInfo.InvariantCulture);
                float lon = float.Parse(node.Attributes["lon"].Value, CultureInfo.InvariantCulture);
                Vector3 point = new Vector3(10f* lat, 0f, 10f* lon);
                roadPoints.Add(point);
            }

            // Dodaj drogę i jej wierzchołki do struktur danych
            roadIds.Add(roadId);
            visibilities.Add(visible);
            roadPointsList.Add(roadPoints);
        }

        // Twórz drogi w Unity na podstawie zebranych danych
        for (int i = 0; i < roadIds.Count; i++)
        {
            CreateRoad(roadIds[i], visibilities[i], roadPointsList[i]);
        }
    }




    void CreateRoad(string roadId, string visible, List<Vector3> roadPoints)
    {
         Debug.Log("roadId: " + roadId + " |  visible: " + visible + " |  roadPoints: ");

        // Wyświetl informacje o punktach drogi
        foreach (Vector3 point in roadPoints)
        {
            Debug.Log("Point: " + point);
        }
        Color lineColor = Color.white;
        // Dodaj komponent LineRenderer
        GameObject roadObject = new GameObject("Road_" + roadId);
        LineRenderer lineRenderer = roadObject.AddComponent<LineRenderer>();

        // Ustaw parametry LineRenderer
        lineRenderer.positionCount = roadPoints.Count;
        lineRenderer.startWidth = 0.001f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // Ustaw tryb mieszania materiału na Opaque
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        // Ustaw pozycje punktów
        lineRenderer.SetPositions(roadPoints.ToArray());

        roadObject.transform.parent = this.transform.parent;
    }
    

 
}
