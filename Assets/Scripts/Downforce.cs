using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downforce : MonoBehaviour {

    public float thrust;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(0, thrust, 0, ForceMode.Force);
    }
}
