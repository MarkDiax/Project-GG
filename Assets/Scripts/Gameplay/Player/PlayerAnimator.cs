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
		Animator.SetFloat("Speed", input.Keyboard.Vertical);
		Animator.SetFloat("Rotation", input.Keyboard.Horizontal);


		if (Input.GetKeyDown(KeyCode.Space)) {

			TriggerExpression("Jump");
		}
	}
}
