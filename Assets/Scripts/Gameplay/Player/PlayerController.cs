using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Player player;
	private InputManager input;

	private Transform cam;
	private Vector3 acceleration;

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

	private void LateUpdate() {
		Quaternion LookDir = transform.rotation;

		if (input.Keyboard.Vertical != 0 || input.Keyboard.Horizontal != 0) {

			float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
			Vector3 playerEuler = transform.rotation.eulerAngles;

			LookDir = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);

			//float Diff = playerEuler.y - MouseY;
			//Debug.Log(Diff);
			//Debug.Log(Quaternion.Angle(transform.rotation, LookDir));
		}

		transform.rotation *= LookDir;
	}
}