using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform : MonoBehaviour {

    public GameObject EnableObject;

    private void OnTriggerEnter(Collider other)
    {
        EnableObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        EnableObject.SetActive(false);
    }
}
