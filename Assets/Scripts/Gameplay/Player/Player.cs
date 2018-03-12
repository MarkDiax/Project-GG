using System;
using UnityEngine;

public class Player : MonoSingleton<Player> {
	private PlayerController _controller;
	private PlayerAnimator _animator;
	private Rigidbody _rigidbody;
    private PlayerActions _actions;

	public PlayerController Controller {
		get {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();
            return _controller; }
	}

	public PlayerAnimator Animator {
		get {
			if (_animator == null)
				_animator = GetComponentInChildren<PlayerAnimator>();
			return _animator;
		}
	}

	public Rigidbody Rigidbody {
		get {
			if (_rigidbody == null)
				_rigidbody = GetComponentInChildren<Rigidbody>();
			return _rigidbody;
		}
	}

    public PlayerActions Actions {
        get {
            if (_actions == null)
                _actions = GetComponentInChildren<PlayerActions>();
            return _actions;
        }
    }
}