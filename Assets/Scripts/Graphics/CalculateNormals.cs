using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateNormals : MonoBehaviour {
    private Mesh _mesh;

    void Awake() {
        _mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update () {
        _mesh.RecalculateNormals(60);
	}
}
