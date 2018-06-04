using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBox : MonoBehaviour {

    private bool canMoveLeft = true;
    private bool canMoveRight = true;
    private bool canMoveForward = true;
    private bool canMoveBackward = true;
    private float speed = 0.01f;
    private Vector3 moveDirectionSide;
    private Vector3 moveDirectionFwd;

    private Vector3 fwd;
    private Vector3 lft;

    private Transform attachedBox;

	
	// Update is called once per frame
	void Update () {


        if (Input.GetKey(KeyCode.D) && canMoveRight == true)
        {
            moveDirectionSide = Vector3.right * speed;
        }
        else if(Input.GetKey(KeyCode.A) && canMoveLeft == true)
        {
            moveDirectionSide = Vector3.left * speed;
        }
        else
        {
            moveDirectionSide = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.W) && canMoveForward == true)
        {
            moveDirectionFwd = Vector3.forward * speed;
        }
        else if (Input.GetKey(KeyCode.S) && canMoveBackward == true)
        {
            moveDirectionFwd = Vector3.forward * -speed;
        }
        else
        {
            moveDirectionFwd = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            attachedBox = transform.Find("MoveBox");
            attachedBox.parent = null;

            transform.GetComponent<Animator>().enabled = true;
            transform.GetComponent<PlayerController>().enabled = true;
            transform.GetComponent<PlayerAnimator>().enabled = true;
            transform.GetComponent<ClimbingController>().enabled = true;
            transform.GetComponent<MoveBox>().enabled = false;

            canMoveBackward = true;
            canMoveForward = true;
            canMoveLeft = true;
            canMoveRight = true;

        }

        transform.Translate(moveDirectionSide + moveDirectionFwd);

    }

    void FixedUpdate()
    {
        //Vector3 fwd = transform.TransformDirection(Vector3.forward);
        //Vector3 lft = transform.TransformDirection(Vector3.left);
        fwd = transform.forward;
        lft = -transform.right;

        if (Physics.Raycast(transform.position, fwd, 3))

        {
            canMoveForward = false;
            Debug.Log("MoveForward" + canMoveForward);
        }
        else
        {
            canMoveForward = true;
            Debug.Log("MoveForward" + canMoveForward);
        }

        if (Physics.Raycast(transform.position, fwd * -1, 1))
            canMoveBackward = false;
        else
        {
            canMoveBackward = true;
        }

        if (Physics.Raycast(transform.position, lft, 1))
            canMoveLeft = false;
        else
        {
            canMoveLeft = true;
        }
        if (Physics.Raycast(transform.position, lft * -1, 1))
            canMoveRight = false;
        else
        {
            canMoveRight = true;
        }
    }
}
