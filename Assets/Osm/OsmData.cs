using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OsmNode{
    public double latitude;
    public double longitude;
    public OsmNode(double latitude, double longitude){
        this.latitude = latitude;
        this.longitude = longitude;
    }

}

public class OsmRoad{
    public string id;
    public List<OsmNode> points;
    public OsmRoad(string id, List<OsmNode> points){
        this.id = id;
        this.points = points;  
    }

}

public class OsmData : MonoBehaviour
{
    private List<OsmRoad> roads;
    public OsmData(){
        roads = new List<OsmRoad>();
    }
    public void AddRoad(OsmRoad road){
        roads.Add(road);
    }
    public List<OsmRoad> GetRoads(){
        return roads;
    }
    public int getRoadsCount(){
        return roads.Count;
    }
}
