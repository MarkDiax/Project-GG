using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float smoothTime = 0.5f;
    public float rotationSpeed = 1f;
	
	// Update is called once per frame
	void Update () {
		{
            //Get Inputs
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

            //Player Rotate
            transform.Rotate(0, x * rotationSpeed, 0);
            //Player Move
            transform.Translate(Vector3.forward * z * 10);
        }

    }
}
