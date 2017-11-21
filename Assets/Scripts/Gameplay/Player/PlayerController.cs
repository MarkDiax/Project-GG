using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float Smoothing;
	public float StartSpeed;
	public float MoveSpeed;
	public float RotationSpeed;

	private InputManager input;

	private Vector3 acceleration;

	private void Awake() {
		input = InputManager.Instance;
	}

	private void Update() {
		/*
		Vector3 SmoothAcceleration = Vector3.Lerp(acceleration, new Vector3(0, 0, input.Keyboard.Vertical * MoveSpeed), Smoothing * Time.deltaTime);

		transform.Translate(SmoothAcceleration);
		transform.Rotate(0, input.Keyboard.Horizontal * RotationSpeed, 0);

		acceleration = SmoothAcceleration;
		*/
	}
}