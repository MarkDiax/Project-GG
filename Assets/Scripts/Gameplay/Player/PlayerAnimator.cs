using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{

    private bool _onRope;
    private Player _player;

    private bool _useRootMotion;


    protected override void Awake() {
        base.Awake();

        _player = Player.Instance;

        _player.Trigger.onRopeTrigger += OnRope;
        _player.Climber.onRopeClimbing += Climb;
    }

    private void Climb(float ClimbSpeed) {
        SetFloat("climbSpeed", ClimbSpeed);
    }

    private void OnRope(RopePart Part) {
        _onRope = Part == null ? false : true;
        _useRootMotion = !_onRope;
    }

    private void Update() {
        Animate();
    }

    private void Animate() {
        SetBool("RopeClimbing", _onRope);
    }

    public void JumpEvent() {
        _player.Controller.Jump();
    }

    private void OnAnimatorMove() {
        if (_useRootMotion)
            transform.position += GetDeltaPosition;
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