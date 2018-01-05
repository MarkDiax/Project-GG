using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	[Tooltip("The amount by which the input is smoothed. Lower value = less responsive")]
	public float InputSmoothing = 9f;

	private Player player;
	private InputManager input;

	private Transform cam;

	private float OldMouseX;
	private bool Grounded;

	public float Speed;
	private float Rotation;

	private Coroutine JumpRoutine;

	private void Awake() {
		cam = Camera.main.transform;
		input = InputManager.Instance;
		player = PlayerTracker.Player;
	}

	private void Update() {
		Move();

		RaycastGround();


		if (Speed != 0 || Rotation != 0) {
			RotateWithMouse();

			//TiltPlayerWithMouse();

			float rotSpeed = Speed >= 0 ? 0 : 180f;
			float Rot = Speed >= 0 ? Rotation * 90f : -Rotation * 90f;

			transform.rotation *= Quaternion.Euler(0f, Rot + rotSpeed, 0f);
			//transform.rotation *= Quaternion.Euler(0f, Speed * transform.eulerAngles.y, 0f);
		}

		UpdateUI();
	}

	private void UpdateUI() {
		player.SpeedUI.text = "Speed: " + Speed;
		player.RotationUI.text = "Rotation " + Rotation;
	}

	private void RaycastGround() {
		RaycastHit Info;
		Physics.Raycast(new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down), out Info, 0.2f);

		Grounded = (Info.collider != null); //&& Info.collider.gameObject.layer == 9);
	}

	private void Move() {
		Speed = Mathf.Lerp(Speed, input.Keyboard.Vertical, InputSmoothing * Time.deltaTime);
		if (Mathf.Abs(Speed) < 0.01f)
			Speed = 0f;

		Rotation = Mathf.Lerp(Rotation, input.Keyboard.Horizontal, InputSmoothing * Time.deltaTime);
		if (Mathf.Abs(Rotation) < 0.01f)
			Rotation = 0f;


		float absoluteSpeed = Mathf.Abs(Speed) + Math.Abs(Rotation);
		transform.Translate(0, 0, (Mathf.Clamp01(absoluteSpeed) * 5) * Time.fixedDeltaTime);
	}

	public void OnAnimatorMove() {
		//apply root motion + physics motion
		transform.position += /*player.Animator.transform.localPosition + */ player.Animator.GetDeltaPosition;
		//transform.rotation = player.Animator.transform.localRotation * player.Animator.GetDeltaRotation;

		//player.Animator.ResetTransform();

	}

	private void FixedUpdate() {
		transform.position += player.Animator.transform.localPosition;

		player.Animator.ResetTransform();
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

		yield return false;
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
		Vector3 PlayerRot = transform.eulerAngles;
		float Tilt = Grounded ? (OldMouseX - input.Mouse.Angle.x + PlayerRot.z) : PlayerRot.z;
		transform.rotation = Quaternion.Euler(PlayerRot.x, PlayerRot.y, Tilt);
		OldMouseX = input.Mouse.Angle.x;
	}

	public float Velocity {
		get {
			return Mathf.Clamp01(Mathf.Abs(Speed) + Mathf.Abs(Rotation));
		}
	}
}
 