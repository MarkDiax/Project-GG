using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour {

	private PlayerController controller;
	private PlayerAnimator animator;


	public PlayerController Controller {
		get {
			if (controller == null)
				controller = GetComponentInChildren<PlayerController>();
			return controller;
		}
	}

	public PlayerAnimator Animator {
		get {
			if (animator == null)
				animator = GetComponentInChildren<PlayerAnimator>();
			return animator;
		}
	}
}