using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {
	public Transform Target;

	public float TargetDistance;
	public float SmoothTime;
	public float ZoomSpeed;

	private InputManager input;

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
		Quaternion Rotation = Quaternion.Euler(input.Mouse.Input.y, input.Mouse.Input.x, 0);

		Vector3 targetPosition = Target.position + (Rotation * Direction);
		transform.position = Vector3.Lerp(transform.position, targetPosition , SmoothTime * Time.deltaTime);


		transform.LookAt(Target);
	}
}