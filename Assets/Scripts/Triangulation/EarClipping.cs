using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    public struct EarClippingProcessData{
        public bool isCounterClockwiseOrder;
        public HashSet<int> convexVertices;
        public HashSet<int> reflexVertices;
        public HashSet<int> earVertices;
    };



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
        way.osmNodes = Enumerable.Range(0, 10).Select(i => (i).ToString()).ToList();
        way.category = "none";
        way.type = "none";
        initTriangulationData(way, nodesDictionary);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 
    public EarClippingProcessData initTriangulationData(OsmWay way,  Dictionary<string, OsmNode> nodesDictionary){
        EarClippingProcessData processData;
        processData.isCounterClockwiseOrder = isCounterClockwiseOrder(way, nodesDictionary);
        processData.convexVertices = new HashSet<int>();
        processData.reflexVertices = new HashSet<int>();
        processData.earVertices = new HashSet<int>();
        Debug.Log(processData.isCounterClockwiseOrder? "Counter-clockwise" : "Clockwise" );

            for(int i = 0; i < way.osmNodes.Count; i++){
                int prev = (i - 1 + way.osmNodes.Count) % way.osmNodes.Count;
                int next = (i + 1 + way.osmNodes.Count) % way.osmNodes.Count;

                Vector3 AM = nodesDictionary[way.osmNodes[prev]].point - nodesDictionary[way.osmNodes[i]].point;
                Vector3 BM = nodesDictionary[way.osmNodes[next]].point - nodesDictionary[way.osmNodes[i]].point;

                float crossY = Vector3.Cross(AM, BM).y;

                if (processData.isCounterClockwiseOrder? (crossY >= 0f ): (crossY < 0f)){
                    processData.convexVertices.Add(i);
                    if(isEar(i, way, nodesDictionary)){
                        processData.earVertices.Add(i);
                    }
                }
                    //Debug.Log("V"+way.osmNodes[i] + " jest wklęsły.");
                else
                    processData.reflexVertices.Add(i); //Debug.Log("V"+way.osmNodes[i] + " jest wypukły.");
            }

            Debug.Log("Convex vertices: "+string.Join(", ", processData.convexVertices));
            Debug.Log("Reflex vertices: "+string.Join(", ", processData.reflexVertices));
            Debug.Log("Ear vertices: "+string.Join(", ", processData.earVertices));
            

        return processData;
    }

    public bool isCounterClockwiseOrder(OsmWay way,  Dictionary<string, OsmNode> nodesDictionary){
        float area = 0f;
        for(int i = 0; i < way.osmNodes.Count; i++){
            int j = (i + 1) % way.osmNodes.Count;
            area += nodesDictionary[way.osmNodes[i]].point.x * nodesDictionary[way.osmNodes[j]].point.z;
            area -= nodesDictionary[way.osmNodes[j]].point.x * nodesDictionary[way.osmNodes[i]].point.z;
        }
        return (area > 0f);
    }

    public bool isEar(int id, OsmWay way,  Dictionary<string, OsmNode> nodesDictionary){
        int previousId = (id-1 + way.osmNodes.Count) % way.osmNodes.Count;
        int nextId = (id+1 + way.osmNodes.Count) % way.osmNodes.Count;
        
        Vector3 v0 = nodesDictionary[way.osmNodes[previousId]].point;
        Vector3 v1 = nodesDictionary[way.osmNodes[id]].point;
        Vector3 v2 = nodesDictionary[way.osmNodes[nextId]].point;

        for (int i = 0; i < way.osmNodes.Count; i++){

            if(i == previousId || i == id || i == nextId){
                continue;
            }
            Vector3 v = nodesDictionary[way.osmNodes[i]].point;


            float d1 = sign(v, v0, v1);
            float d2 = sign(v, v1, v2);
            float d3 = sign(v, v2, v0);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            if(!(hasNeg && hasPos))
                return false;
            

        }
        return true;
    }


    private float sign(Vector3 v0, Vector3 v1, Vector3 v2){
        return (v0.x - v2.x) * (v1.z - v2.z) - (v1.x - v2.x) * (v0.z - v2.z);
    }

}
