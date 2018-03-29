using System;
using UnityEngine;

public class Player : MonoSingleton<Player>
{
    private PlayerController _controller;
    private PlayerCombat _combat;
    private ClimbingController _climber;
    private PlayerAnimator _animator;
    private Rigidbody _rigidbody;
    private PlayerTrigger _trigger;


    public PlayerController Controller {
        get {
            if (_controller == null)
                _controller = GetComponentInChildren<PlayerController>();
            return _controller;
        }
    }

    public PlayerCombat Combat {
        get {
            if (_combat == null)
                _combat = GetComponentInChildren<PlayerCombat>();
            return _combat;
        }
    }

    public ClimbingController Climber {
        get {
            if (_climber == null)
                _climber = GetComponentInChildren<ClimbingController>();
            return _climber;
        }
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

    public PlayerTrigger Trigger {
        get {
            if (_trigger == null)
                _trigger = GetComponentInChildren<PlayerTrigger>();
            return _trigger;
        }
    }
}