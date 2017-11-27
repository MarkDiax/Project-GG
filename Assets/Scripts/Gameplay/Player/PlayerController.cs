using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private InputManager input;
	private Player player;

	private Transform cam;
	private Vector3 acceleration;

	private void Awake() {
		input = InputManager.Instance;
		cam = Camera.main.transform;

		player = PlayerTracker.Player;
	}

	public void OnAnimatorMove() {
		transform.position += player.Animator.Animator.deltaPosition;
	}

	private void Update() {
		
		if (input.Keyboard.Vertical != 0) {

			float MouseY = Quaternion.LookRotation(cam.transform.forward).eulerAngles.y;
			Vector3 playerEuler = player.transform.localEulerAngles;
			player.transform.localRotation = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
		}

	
	}
}