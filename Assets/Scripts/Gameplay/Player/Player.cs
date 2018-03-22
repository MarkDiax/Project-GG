﻿using System;
using UnityEngine;

public class Player : MonoSingleton<Player>
{
    private PlayerController _controller;
    private ClimbingController _climber;
    private PlayerAnimator _animator;
    private Rigidbody _rigidbody;
    private PlayerTrigger _trigger;

    //private PlayerActions _actions;

    public PlayerController Controller {
        get {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();
            return _controller;
        }
    }

    public ClimbingController Climber {
        get {
            if (_climber == null)
                _climber = GetComponent<ClimbingController>();
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

    //public PlayerActions Actions {
    //    get {
    //        if (_actions == null)
    //            _actions = GetComponentInChildren<PlayerActions>();
    //        return _actions;
    //    }
    //}
}