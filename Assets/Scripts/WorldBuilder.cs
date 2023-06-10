using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WorldBuilder : MonoBehaviour
{
    [SerializeField]
    private OsmClient osmClient;


    public float scale = 1000f;

    public float lineWidth = 0.001f; 

    GameObject highways; 
    GameObject buildings; 
    GameObject railways; 
    GameObject naturals; 
    GameObject landuses; 
    GameObject others; 

    [SerializeField]
    private EarClipping earClipping;


    private float offset_x = 0f;
    private float offset_z = 0f;

    void Start()
    {
        highways = new GameObject("Highways");
        highways.transform.parent = this.transform.parent;

        buildings = new GameObject("Buildings");
        buildings.transform.parent = this.transform.parent;

        railways = new GameObject("Railways");
        railways.transform.parent = this.transform.parent;

        naturals = new GameObject("Naturals");
        naturals.transform.parent = this.transform.parent;

        landuses = new GameObject("Landuses");
        landuses.transform.parent = this.transform.parent;

        others = new GameObject("Others");
        others.transform.parent = this.transform.parent; 

        OsmData chunkData = osmClient.readOsmFile(Application.dataPath + "/Osm/map.xml");
        
        offset_x = chunkData.osmBounds.minLatitude;
        offset_z = chunkData.osmBounds.minLongitude;
        Debug.Log("Offset = x: "+offset_x+" y: "+offset_z);

        generateChunk(chunkData);


    }

    
    void Update(){
        
    }


    void generateChunk(OsmData chunkData){
        DateTime Start = DateTime.Now;

        buildBounds(chunkData.osmBounds);


        foreach (KeyValuePair<string, OsmWay> way in chunkData.waysDictionary){
            List<Vector3> points = new List<Vector3>();
            foreach(string nodeId in way.Value.osmNodes){
                if(chunkData.nodesDictionary.ContainsKey(nodeId))
                    points.Add((chunkData.nodesDictionary[nodeId].point - new Vector3(offset_x, 0f, offset_z))* scale);
            }

            switch(way.Value.category){
                case "highway": 
                    buildWithLines("highway_"+way.Key, points, Color.green, highways);
                    break;
                case "building": 
                    buildWithLines("building_"+way.Key, points, new Color(0.5f, 1f, 0f, 1f), buildings);
                    break;
                case "railway": 
                    buildWithLines("railway_"+way.Key, points, Color.yellow, railways);
                    break;
                case "natural": 
                    buildNaturals(way.Key, points.GetRange(0, points.Count - 1), way.Value.type);
                    break;
                case "landuse": 
                    buildLanduses(way.Key, points.GetRange(0, points.Count - 1), way.Value.type);
                    break;    
                default:
                    buildOthers(way.Value.category + way.Key, points);
                    break;
            }

        }

        Debug.Log("Chunk generation process completed - elapsed time: " + DateTime.Now.Subtract(Start).ToString());
    }
 

    void buildBounds(OsmBounds osmBounds){
        GameObject bounds = new GameObject("chunk");

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(osmBounds.minLatitude * scale, 0f, osmBounds.minLongitude * scale); 
        vertices[1] = new Vector3(osmBounds.maxLatitude * scale, 0f, osmBounds.minLongitude * scale); 
        vertices[2] = new Vector3(osmBounds.minLatitude * scale, 0f, osmBounds.maxLongitude * scale); 
        vertices[3] = new Vector3(osmBounds.maxLatitude * scale, 0f, osmBounds.maxLongitude * scale);
        
        int[] triangles = new int[6] { 0, 2, 1, 1, 2, 3 };

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;    
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            

            MeshFilter meshFilter = bounds.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            
            MeshRenderer meshRenderer = bounds.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = Color.white;

    }

    void buildWithLines(string id, List<Vector3> points, Color lineColor, GameObject parent){
            GameObject newGameObject = new GameObject(id);
            LineRenderer lineRenderer = newGameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.Select(p => new Vector3(p.x, 0.01f, p.z)).ToArray());
            newGameObject.transform.parent = parent.transform;
    }


    void buildNaturals(string id, List<Vector3> points, string type){
             if(type == "water"){
                buildMesh("water_"+id, 0.006f, points, Color.blue, naturals);
            } else if(type == "scrub"){
                buildMesh("scrub_"+id, 0.005f, points, new Color(0.047f, 0.553f, 0.478f, 1.0f), naturals);
            } else if(type == "wood"){
                buildMesh("wood_"+id, 0.007f, points, new Color(0.035f, 0.863f, 0.659f, 1.0f), naturals);
            } else if(type == "grassland"){
                buildMesh("grassland_"+id, 0f, points, new Color(0.808f, 0.925f, 0.694f, 1.0f), naturals);
            } else if(type == "heath"){
                buildMesh("heath"+id, 0f, points, new Color(0.839f, 0.851f, 0.624f, 1.0f), naturals);
            } else {
                buildOthers(type+id, points);
            } 
    }


    void buildLanduses(string id, List<Vector3> points, string type){
             if(type == "residential"){
                buildMesh("residential"+id, 0.009f, points, new Color(0.055f, 0.118f, 0.118f, 1.0f), landuses);
            } else  if(type == "landfill"){
                buildMesh("landfill"+id, 0.003f, points, new Color(0.714f, 0.714f, 0.565f, 1.0f), landuses);
            } else  if(type == "meadow"){
                buildMesh("meadow"+id, 0.002f, points, new Color(0.804f, 0.922f, 0.69f, 1.0f), landuses);   
            } else  if(type == "industrial"){
                buildMesh("industrial"+id, 0.004f, points, new Color(0.922f, 0.859f, 0.914f, 1.0f), landuses); 
            } else if(type =="forest"){
                buildMesh("forest"+id, 0.008f, points, new Color(0.616f, 0.792f, 0.541f, 1.0f), landuses); 
            } else if(type =="farmland"){
                buildMesh("farmland"+id, 0.001f, points, new Color(0.933f, 0.941f, 0.835f, 1.0f), landuses); 
            } else if(type =="grass"){
                buildMesh("grass"+id, 0.0f, points, new Color(0.808f, 0.925f, 0.694f, 1.0f), landuses); 
            } else if(type =="education"){
                buildMesh("education"+id, 0.010f, points, new Color(0.549f, 0.263f, 0.039f, 1.0f), landuses); 
            }
            else {
                buildOthers(type+id, points);
            } 




    }



    void buildMesh(string id, float offset_y, List<Vector3> points, Color meshColor, GameObject parent){
           GameObject newGameObject = new GameObject(id);
//Debug.Log("Build : "+id);
                Mesh mesh = new Mesh();
                Vector3[] vertices = points.Select(p => new Vector3(p.x, offset_y, p.z)).ToArray();
                mesh.vertices = vertices;

                mesh.triangles = earClipping.triangulation(points).ToArray();

                mesh.RecalculateNormals();
                Vector3[] normals = mesh.normals;

                for (int i = 0; i < normals.Length; i++){
                    normals[i] = Vector3.up;
                }
                mesh.normals = normals;

                MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                
                MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = meshColor;
                
                newGameObject.transform.parent = parent.transform;
    }

    void buildOthers(string id, List<Vector3> points){
            GameObject other = new GameObject(id);

            LineRenderer lineRenderer = other.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPositions(points.ToArray());
            other.transform.parent = others.transform;
    }




}
