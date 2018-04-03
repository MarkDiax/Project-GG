﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateNormals : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals(60);
	}
}
