using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Player player;
	private InputManager input;

	private Transform cam;
	private Vector3 acceleration;

	private float OldMouseX;

	private void Awake() {
		cam = Camera.main.transform;
		input = InputManager.Instance;
		player = PlayerTracker.Player;
	}

	public void OnAnimatorMove() {
		//apply root motion + physics motion

		transform.position += player.Animator.GetDeltaPosition + player.Animator.transform.localPosition;
		transform.rotation = player.Animator.GetDeltaRotation * player.Animator.transform.localRotation;
	}

	public void Jump(float Height) {
		if (Grounded())
			GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, Height * 10, 0), ForceMode.Impulse);
	}

	private bool Grounded() {
		return (!Physics.Raycast(new Ray(transform.position, Vector3.down), 0.1f, 9));
	}

	private void FixedUpdate() {
		Quaternion TargetRot = transform.localRotation;

		if (Grounded()) {
			float Tilt = (OldMouseX - input.Mouse.Angle.x) * 4;
			Vector3 PlayerRot = transform.rotation.eulerAngles;
			TargetRot = Quaternion.Euler(new Vector3(PlayerRot.x, PlayerRot.y, Tilt));

			transform.rotation = TargetRot;//Quaternion.Lerp(transform.localRotation, TargetRot, 5 *Time.deltaTime);
		}

		OldMouseX = input.Mouse.Angle.x;
	}

	private void LateUpdate() {
		Quaternion LookDir = transform.rotation;

		if (input.Keyboard.Vertical != 0 || input.Keyboard.Horizontal != 0) {

			float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
			Vector3 playerEuler = transform.rotation.eulerAngles;

			LookDir = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
		}

		transform.rotation *= LookDir;
	}
}