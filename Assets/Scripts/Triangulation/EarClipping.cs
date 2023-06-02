using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EarClipping : MonoBehaviour
{


     public Dictionary<string, OsmNode> nodesDictionary;

    void Start()
    {

        nodesDictionary = new Dictionary<string, OsmNode>();
        OsmNode p0;
        p0.point =  new Vector3(2.5f, 0f, 5f);
        nodesDictionary.Add("0", p0);
        
        OsmNode p1;
        p1.point =  new Vector3(5.75f, 0f, 2.5f);
        nodesDictionary.Add("1", p1);

        OsmNode p2;
        p2.point =  new Vector3(9f, 0f, 5.25f);
        nodesDictionary.Add("2", p2);
        
        OsmNode p3;
        p3.point =  new Vector3(11.5f, 0f, 3.5f);
        nodesDictionary.Add("3", p3);
        
        OsmNode p4;
        p4.point =  new Vector3(14f, 0f, 7f);
        nodesDictionary.Add("4", p4);
        
        OsmNode p5;
        p5.point =  new Vector3(11f, 0f, 6.75f);
        nodesDictionary.Add("5", p5);
        
        OsmNode p6;
        p6.point =  new Vector3(9.75f, 0f, 9.5f);
        nodesDictionary.Add("6", p6);
        
        OsmNode p7;
        p7.point =  new Vector3(7f, 0f, 4.75f);
        nodesDictionary.Add("7", p7);
        
        OsmNode p8;
        p8.point =  new Vector3(4f, 0f, 5.5f);
        nodesDictionary.Add("8", p8);
        
        OsmNode p9;
        p9.point =  new Vector3(4.25f, 0f, 8.5f);
        nodesDictionary.Add("9", p9);

        OsmWay way;
        way.osmNodes = Enumerable.Range(0, 10).Select(i => (9-i).ToString()).ToList();
        way.category = "none";
        way.type = "none";

if(isClockwiseOrder(way, nodesDictionary)){
    Debug.Log("Clockwise");
} else {
    Debug.Log("CounterClockwise");
}
        
        getTriangles(way, nodesDictionary);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getTriangles(OsmWay way, Dictionary<string, OsmNode> nodesDictionary){
        for(int i = 0; i < way.osmNodes.Count; i++){

            int prev = (i - 1 + way.osmNodes.Count) % way.osmNodes.Count;
            int next = (i + 1 + way.osmNodes.Count) % way.osmNodes.Count;

            Vector3 AM = nodesDictionary[way.osmNodes[prev]].point - nodesDictionary[way.osmNodes[i]].point;
            Vector3 BM = nodesDictionary[way.osmNodes[next]].point - nodesDictionary[way.osmNodes[i]].point;

            if (Vector3.Cross(AM, BM).y <= 0f)
                Debug.Log("V"+way.osmNodes[i] + " jest wklęsły.");
            else
                Debug.Log("V"+way.osmNodes[i] + " jest wypukły.");
            
          

        }
        
    }

    public bool isClockwiseOrder(OsmWay way,  Dictionary<string, OsmNode> nodesDictionary){
        float area = 0f;
        for(int i = 0; i < way.osmNodes.Count; i++){
            int j = (i + 1) % way.osmNodes.Count;
            area += nodesDictionary[way.osmNodes[i]].point.x * nodesDictionary[way.osmNodes[j]].point.z;
            area -= nodesDictionary[way.osmNodes[j]].point.x * nodesDictionary[way.osmNodes[i]].point.z;
        }
        return (area > 0f);
    }


}
