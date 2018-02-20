using System;
using UnityEngine;

public class Player : MonoBehaviour {
	private PlayerController controller;
	private PlayerAnimator animator;
	private new Rigidbody rigidbody;

	public PlayerController Controller {
		get {
            if (controller == null)
                controller = GetComponent<PlayerController>();
            return controller; }
	}

	public PlayerAnimator Animator {
		get {
			if (animator == null)
				animator = GetComponentInChildren<PlayerAnimator>();
			return animator;
		}
	}

	public Rigidbody Rigidbody {
		get {
			if (rigidbody == null)
				rigidbody = GetComponentInChildren<Rigidbody>();
			return rigidbody;
		}
	}
}