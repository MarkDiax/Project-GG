using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private Player player;
	private InputManager input;

	private Transform cam;
	private Vector3 acceleration;

	private float OldMouseX;
	private bool Grounded;

	private Quaternion LookDir;

	private Coroutine JumpRoutine;

	private void Awake() {
		cam = Camera.main.transform;
		input = InputManager.Instance;
		player = PlayerTracker.Player;
	}

	private void Update() {
		RaycastHit Info;
		Physics.Raycast(new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down), out Info, 0.2f);

		Grounded = (Info.collider != null); //&& Info.collider.gameObject.layer == 9);
	}

	public void OnAnimatorMove() {
		//apply root motion + physics motion
		transform.position += player.Animator.transform.localPosition + player.Animator.GetDeltaPosition;
		transform.rotation = player.Animator.transform.localRotation * player.Animator.GetDeltaRotation;

		player.Animator.ResetTransform();

		if (input.Keyboard.Vertical != 0 || input.Keyboard.Horizontal != 0) {
			//TiltPlayerWithMouse();

			RotateWithMouse();
		}
	}

	public void Jump() {
		if (Grounded && JumpRoutine == null)
			JumpRoutine = StartCoroutine(UpdateJumpStates());
	}

	private IEnumerator UpdateJumpStates() {
		player.Rigidbody.useGravity = false;

		yield return Jump_Start(player.JumpValues.Height, player.JumpValues.Attack);
		yield return Jump_End(player.JumpValues.Height, player.JumpValues.Decay);

		player.Rigidbody.useGravity = true;
		JumpRoutine = null;
	}

	private IEnumerator Jump_Start(float Height, float Attack) {
		float Progress = 0f;

		player.transform.position += new Vector3(0, player.JumpValues.StartHeight, 0);

		while (Progress <= player.JumpValues.Height) {

			Progress += player.JumpValues.Attack;
			transform.position += new Vector3(0, player.JumpValues.Attack, 0);

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator Jump_End(float Height, float Decay) {
		float Progress = Height;

		while (Progress >= 0) {
			if (Grounded)
				yield break;

			Progress -= Decay;
			transform.position -= new Vector3(0, Decay, 0);

			yield return new WaitForEndOfFrame();
		}

		transform.position -= new Vector3(0, player.JumpValues.StartHeight, 0);
	}

	private void RotateWithMouse() {
		float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
		Vector3 playerEuler = transform.eulerAngles;
		transform.rotation = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
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