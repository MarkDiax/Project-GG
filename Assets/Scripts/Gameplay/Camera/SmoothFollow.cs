using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    //rotation
    public Transform cameraObj;
    public Transform target;
    public GameObject CameraFollowObj;
    public float CameraMoveSpeed = 120.0f;
    public float clampAngle = 50.0f;
    public float inputSensitivity = 150.0f;
    private float mouseX;
    private float mouseY;
    private float finalInputX;
    private float finalInputZ;
    private float rotY = 45.0f;
    private float rotX = 0.0f;

    //translation
    public Vector3 offset;
    public float smoothSpeed = 0.2f;



    // Use this for initialization
    void Awake()
    {
        //Set Camera Distance from player
        Vector3 CameraDistance = cameraObj.position + offset;
        cameraObj.position = CameraDistance;

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Smooth Follow Player, Update each frame
        transform.position = Vector3.Lerp(transform.position, target.position, 10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // We setup the rotation of the sticks here
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");
        //Set up Keys
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        //Combine inputs
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        //Rotation Speed
        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;

        //Camera Angle limit
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        //Rotate Camera
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, localRotation, smoothSpeed) ;

        //Make Camera-child look at player
        cameraObj.LookAt(target);


    }

}
