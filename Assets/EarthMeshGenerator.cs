using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthMeshGenerator : MonoBehaviour
{

    [Range(2,256)]
    public int resolution = 10;
   
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;



    public bool cube = true;

    //whenever we update anything in editor  
    void OnValidate(){
        Initialize();
        GenerateMesh();
    }


  void Initialize(){

        //we don't nwat do create a new set of six mesh filters each time his gets initialized
        if(meshFilters == null || meshFilters.Length == 0){
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for(int i =0; i < 6; i++){
            if(meshFilters[i] == null){
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i], cube);
        }
    }


    void GenerateMesh(){
        foreach(TerrainFace face in terrainFaces){
            face.ConstructMesh();
        }
    }


}
