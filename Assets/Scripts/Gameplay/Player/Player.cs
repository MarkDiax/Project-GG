using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class JumpValues {
	public float StartHeight;
	public float Attack;
	public float Height;
	public float Decay;
}


[System.Serializable]
public class MovementValues {
	public float Acceleration;
	public float MaxSpeed;
}

public class Player : MonoBehaviour {

	public Text SpeedUI;
	public Text RotationUI;

	public JumpValues JumpValues;
	public MovementValues MovementValues;

	private PlayerController controller;
	private PlayerAnimator animator;

	private new Rigidbody rigidbody;

	private void Awake() {
		controller = gameObject.AddComponent<PlayerController>();
	}

	public PlayerController Controller {
		get { return controller; }
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