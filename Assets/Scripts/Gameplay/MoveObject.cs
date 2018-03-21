using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    public float MinClamp = 30f;
    public float MaxClamp = 150f;
	
	// Update is called once per frame
	void LateUpdate () {
        transform.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, MinClamp, MaxClamp),transform.rotation.eulerAngles.y, Mathf.Clamp(transform.rotation.eulerAngles.z, MinClamp, MaxClamp));
    }
}
