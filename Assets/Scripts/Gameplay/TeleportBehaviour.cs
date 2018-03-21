using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBehaviour : MonoBehaviour {

    public GameObject endPoint;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = endPoint.transform.position;
    }
}
