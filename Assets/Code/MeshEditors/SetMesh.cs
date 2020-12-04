using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMesh : MonoBehaviour {

    public ChangeMesh changer;

    [System.Serializable]
    public struct Meshes {
        public Mesh mesh;
    }
    public Meshes[] meshes;


    public void ChangeMesh(int to) {
        changer.differentStart = true;
        GetComponent<MeshFilter>().sharedMesh = meshes[to].mesh;
        if (changer != null) {
            changer.on = to;
        }
    }
}
