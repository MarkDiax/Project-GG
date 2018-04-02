using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    public GameObject snapPoint;
    public GameObject moveableCrate;
    public GameObject player;
    private bool attached;

    private void Start()
    {
        attached = false;
    }

    private void Update()
    {
        if (attached == true && Input.GetKey("e"))
        {
            moveableCrate.transform.parent = null;
            attached = false;
            Debug.Log("Detached");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        player.transform.position = snapPoint.transform.position;
        player.transform.eulerAngles = new Vector3(0, snapPoint.transform.rotation.y, 0);
        moveableCrate.transform.parent = player.transform;
        attached = true;
        Debug.Log("Attached");
    }
}
