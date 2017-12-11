using System;
using UnityEngine;

[System.Serializable]
public class JumpValues {
	public float StartHeight;
	public float Attack;
	public float Height;
	public float Decay;
}

public class Player : MonoBehaviour {

	public JumpValues JumpValues;

	private PlayerController controller;
	private PlayerCharacter animator;

	private new Rigidbody rigidbody;

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