using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour {

	private PlayerController controller;
	private PlayerCharacter animator;

	private Rigidbody rigidbody;

	private void Awake() {
		controller = gameObject.AddComponent<PlayerController>();
	}

	public PlayerController Controller {
		get { return controller; }
	}

	public PlayerCharacter Animator {
		get {
			if (animator == null)
				animator = GetComponentInChildren<PlayerCharacter>();
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