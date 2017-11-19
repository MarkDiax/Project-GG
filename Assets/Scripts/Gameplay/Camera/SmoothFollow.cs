using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {
	//rotation
	private Transform _camera;
	//public Transform target;

	//public float CameraMoveSpeed = 120.0f;
	//public float clampAngle = 50.0f;
	//public float inputSensitivity = 150.0f;
	//private float mouseX;
	//private float mouseY;
	//private float finalInputX;
	//private float finalInputZ;
	//private float rotY = 45.0f;
	//private float rotX = 0.0f;

	////translation
	//public Vector3 offset;
	//public float smoothSpeed = 0.2f;

	//void Awake() {
	//	_camera = Camera.main.transform;

	//	//Set Camera Distance from player
	//	Vector3 CameraDistance = _camera.position + offset;
	//	_camera.position = CameraDistance;

	//	Vector3 rot = transform.localRotation.eulerAngles;
	//	rotY = rot.y;
	//	rotX = rot.x;
	//}


	public Transform target;
	public float smoothTime = 0.3F;
	private Vector3 velocity = Vector3.zero;

	void Update() {
		Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

		transform.LookAt(target);
	}

	//private void Update() {
	//	transform.position = Vector3.Lerp(transform.position, target.position, 10);
	//}

	//void FixedUpdate() {
	//	// We setup the rotation of the sticks here
	//	float inputX = Input.GetAxis("RightStickHorizontal");
	//	float inputZ = Input.GetAxis("RightStickVertical");
	//	//Set up Keys
	//	mouseX = Input.GetAxis("Mouse X");
	//	mouseY = Input.GetAxis("Mouse Y");
	//	//Combine inputs
	//	finalInputX = inputX + mouseX;
	//	finalInputZ = inputZ + mouseY;

	//	//Rotation Speed
	//	rotY += finalInputX * inputSensitivity * Time.deltaTime;
	//	rotX += finalInputZ * inputSensitivity * Time.deltaTime;

	//	//Camera Angle limit
	//	rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

	//	//Rotate Camera
	//	Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
	//	transform.rotation = Quaternion.Lerp(transform.rotation, localRotation, smoothSpeed);

	//	//Make Camera-child look at player
	//	_camera.LookAt(target);
	//}
}