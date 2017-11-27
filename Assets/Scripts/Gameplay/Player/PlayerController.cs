using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float Smoothing;
	public float StartSpeed;
	public float MoveSpeed;
	public float RotationSpeed;

	private InputManager input;
	private PlayerAnimator player;

	private Transform cam;
	private Vector3 acceleration;

	private void Awake() {
		input = InputManager.Instance;
		cam = Camera.main.transform;

		player = GetComponentInChildren<PlayerAnimator>();
	}


	private void LateUpdate() {

		//if (input.Keyboard.Vertical != 0) {

		//	float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
		//	Vector3 playerEuler = player.transform.localEulerAngles;
		//	player.transform.localRotation = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
		//}
	}

	private void OnDrawGizmos() {

		Vector3 S = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
		Gizmos.DrawRay(player.transform.position, cam.transform.forward);
	}

	private void Update() {

		player.SetFloat("Speed", input.Keyboard.Vertical);
		player.SetFloat("Rotation", input.Keyboard.Horizontal);


		if (Input.GetKeyDown(KeyCode.Space)) {

			player.TriggerExpression("Jump");
		}


		/*
		Vector3 SmoothAcceleration = Vector3.Lerp(acceleration, new Vector3(0, 0, input.Keyboard.Vertical * MoveSpeed), Smoothing * Time.deltaTime);

		transform.Translate(SmoothAcceleration);
		transform.Rotate(0, input.Keyboard.Horizontal * RotationSpeed, 0);

		acceleration = SmoothAcceleration;
		*/
	}
}