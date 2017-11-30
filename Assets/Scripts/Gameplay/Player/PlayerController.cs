using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Player player;
	private InputManager input;

	private Transform cam;
	private Vector3 acceleration;
	private Quaternion prevRotation;

	private void Awake() {
		cam = Camera.main.transform;
		input = InputManager.Instance;
		player = PlayerTracker.Player;
	}

	public void OnAnimatorMove() {
		transform.position += player.Animator.GetDeltaPosition;
		transform.rotation = player.Animator.GetDeltaRotation;
	}
	
	private void Update() {
		float Horizontal = input.Keyboard.Horizontal;
		float Vertical = input.Keyboard.Vertical;

		Vector3 CamForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1).normalized);
		Vector3 Move = Vertical * CamForward + Horizontal * cam.right;

		Move = transform.InverseTransformDirection(Move);

		
	}

	private void LateUpdate() {
		Quaternion Updated = transform.rotation;

		if (input.Keyboard.Vertical != 0 || input.Keyboard.Horizontal != 0) {
			prevRotation = transform.rotation;

			float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
			Vector3 playerEuler = transform.rotation.eulerAngles;
			Updated = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
		}
		else 
			Updated = prevRotation;

		transform.rotation *= Updated;
	}
}