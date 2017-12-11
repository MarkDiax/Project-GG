using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharacter : BaseAnimator {
	[Tooltip("The amount by which the input is smoothed. Lower value = less responsive")]
	public float SmoothingValue;
	public float JumpHeight;

	private Player player;
	private InputManager input;

	private float speed;
	private float rotation;

	protected override void Awake() {
		base.Awake();

		player = PlayerTracker.Player;
		input = InputManager.Instance;
	}

	private void Update() {
		Animate();
	}

	private void Animate() {
		speed = Mathf.Lerp(speed, input.Keyboard.Vertical, SmoothingValue * Time.deltaTime);
		rotation = Mathf.Lerp(rotation, input.Keyboard.Horizontal, SmoothingValue * Time.deltaTime);

		SetFloat("Speed", speed);
		SetFloat("Rotation", rotation);

		if (Input.GetKeyDown(KeyCode.Space)) {
			player.Controller.Jump(JumpHeight);
		}
		//if (Input.GetKeyDown(KeyCode.Space)) {
		//	TriggerExpression("Jump");
		//}
	}

	private void OnAnimatorMove() {
		player.Controller.OnAnimatorMove();

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	public Vector3 GetDeltaPosition {
		get { return Animator.deltaPosition; }
	}

	public Quaternion GetDeltaRotation {
		get { return Animator.deltaRotation; }
	}
}
