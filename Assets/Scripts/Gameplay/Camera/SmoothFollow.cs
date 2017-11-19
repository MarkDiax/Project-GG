using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {
	public Transform Target;

	public float TargetDistance;
	public float SmoothTime;
	public float ZoomSpeed;
	public Vector3 CameraOffset = new Vector3(0, 5, 0);

	private InputManager input;
	private Vector3 velocity = Vector3.zero;

	private void Start() {
		input = InputManager.Instance;
	}

	void Update() {
		if (input.Mouse.ScrollWheel > 0)
			TargetDistance += ZoomSpeed;
		else if (input.Mouse.ScrollWheel < 0)
			TargetDistance -= ZoomSpeed;

		TargetDistance = Mathf.Clamp(TargetDistance, 4f, 30f);

		Vector3 Direction = new Vector3(0, 0, -TargetDistance);
		Quaternion Rotation = Quaternion.Euler(input.Mouse.Angle.y, input.Mouse.Angle.x, 0);

		Vector3 targetPosition = Target.position + (Rotation * Direction);
		transform.position = Vector3.Lerp(transform.position, targetPosition , SmoothTime * Time.deltaTime);


		transform.LookAt(Target);
	}
}