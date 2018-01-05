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
		Animate();
	}

	private void Animate() {

		SetFloat("Speed", player.Controller.Velocity);
		//SetFloat("Rotation", rotation);

		if (Input.GetKeyDown(KeyCode.Space)) {
			player.Controller.Jump();
		}
	}

	private void OnAnimatorMove() {	
		player.Controller.OnAnimatorMove();
	}

	public void ResetTransform() {
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