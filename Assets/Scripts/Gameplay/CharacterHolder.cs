using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHolder : MonoBehaviour
{
    public bool IsExitTrigger;
       
    private void OnTriggerEnter(Collider other)
    {
        if(IsExitTrigger == false)
        {
            other.transform.parent = gameObject.transform;
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if(IsExitTrigger == true)
        {
            other.transform.parent = null;
        }
    }

}