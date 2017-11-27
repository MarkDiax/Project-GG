using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : BaseAnimator {

	private Player player;
	private InputManager input;

	protected override void Awake() {
		base.Awake();

		player = PlayerTracker.Player;
		input = InputManager.Instance;
	}

	private void Update() {
		SetFloat("Speed", input.Keyboard.Vertical);
		SetFloat("Rotation", input.Keyboard.Horizontal);

		if (Input.GetKeyDown(KeyCode.Space)) {

			TriggerExpression("Jump");
		}
	}

	private void OnAnimatorMove() {

		player.Controller.OnAnimatorMove();

		//Animator.ApplyBuiltinRootMotion();


		////if (input.Keyboard.Vertical != 0) {

		//	float MouseY = Quaternion.LookRotation(Camera.main.transform.forward).eulerAngles.y;
		//	Vector3 playerEuler = transform.localEulerAngles;
		//	transform.localRotation = Quaternion.Euler(playerEuler.x, MouseY, playerEuler.z);
		////}
	}
}
