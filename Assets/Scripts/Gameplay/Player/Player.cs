using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour {

	private PlayerController controller;
	private PlayerCharacter animator;


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
}