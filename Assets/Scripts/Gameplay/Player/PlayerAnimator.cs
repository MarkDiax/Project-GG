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
		//SetFloat("Rotation", input.Keyboard.Horizontal);

		if (Input.GetKeyDown(KeyCode.Space)) {
			TriggerExpression("Jump");
		}
	}

	private void OnAnimatorMove() {
		player.Controller.OnAnimatorMove();
	}
}
