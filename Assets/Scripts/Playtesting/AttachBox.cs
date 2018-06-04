using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachBox : MonoBehaviour {

    public Transform player;
    public Transform transformPoint;
    public int pRotation;
    public GameObject Playersc;

    private bool triggerEnter = false;


	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.E) && triggerEnter == true)
        {
            player.transform.position = transformPoint.transform.position;
            player.eulerAngles = new Vector3(0, pRotation, 0);
            transform.root.parent = player.transform;
            Playersc.GetComponent<Animator>().enabled = false;
            Playersc.GetComponent<PlayerController>().enabled = false;
            Playersc.GetComponent<PlayerAnimator>().enabled = false;
            Playersc.GetComponent<ClimbingController>().enabled = false;
            Playersc.GetComponent<MoveBox>().enabled = true;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        triggerEnter = false;
    }
}
