using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform : MonoBehaviour {

    public GameObject EnableObject;
    public Transform Box;

    private void Update()
    {
        float dist = Vector3.Distance(Box.position, transform.position);

        if(dist <= 1f)
        {
            EnableObject.SetActive(true);
        }
        else
        {
            EnableObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnableObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        EnableObject.SetActive(false);
    }
}
