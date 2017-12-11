using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Player player;
	private InputManager input;

	private Transform cam;
	private Vector3 acceleration;

	private float OldMouseX;
	private bool Grounded;

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
		if (Grounded)
			player.Rigidbody.AddForce(new Vector3(0, Height, 0), ForceMode.VelocityChange);
	}

	private void Update() {
		RaycastHit Info;
		Physics.Raycast(new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down), out Info, 0.2f);

		Grounded = (Info.collider != null); //&& Info.collider.gameObject.layer == 9);
	}

	private void LateUpdate() {
		Quaternion LookDir = transform.rotation;

		if (input.Keyboard.Vertical != 0 || input.Keyboard.Horizontal != 0) {

			//TiltPlayerWithMouse();

			float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
			Vector3 playerEuler = transform.rotation.eulerAngles;

			LookDir = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
		}

		transform.rotation *= LookDir;
	}


	private void TiltPlayerWithMouse() {
		Quaternion TargetRot = transform.localRotation;

		if (Grounded) {
			float Tilt = (OldMouseX - input.Mouse.Angle.x) * 4;

			Vector3 PlayerRot = transform.localEulerAngles;
			TargetRot = Quaternion.Euler(new Vector3(PlayerRot.x, PlayerRot.y, Tilt));
		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetRot, 5 * Time.deltaTime);

		OldMouseX = input.Mouse.Angle.x;
	}
}