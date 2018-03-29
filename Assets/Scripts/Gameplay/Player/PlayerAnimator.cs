using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    private Player _player;

    private bool _onRope;
    private bool _useRootMotion;

    [Header("Bones")]
    [SerializeField]
    private Transform _spine;

    protected override void Awake() {
        base.Awake();

        _player = Player.Instance;
    }

    private void Start() {
        EventManager.RopeEvent.OnRope.AddListener(OnRope);
        EventManager.RopeEvent.OnRopeClimbing.AddListener(Climb);
    }

    private void Climb(float ClimbSpeed) {
        SetFloat("climbSpeed", ClimbSpeed);
    }

    private void OnRope(bool OnRope) {
        _onRope = OnRope;
        _useRootMotion = !_onRope;
    }

    public void OnRopeClimb() {
        if (InputManager.GetAxis(InputKey.MoveVertical) > 0) {
            if (EventManager.RopeEvent.OnHandSwitch != null)
                EventManager.RopeEvent.OnHandSwitch();

        }
        else {
            if (EventManager.RopeEvent.OnRopeHold != null)
                EventManager.RopeEvent.OnRopeHold();
        }
    }

    public void OnRopeHold() {
        if (InputManager.GetAxis(InputKey.MoveVertical) > 0) {
            if (EventManager.RopeEvent.OnRopeHold != null)
                EventManager.RopeEvent.OnRopeHold();
        }
        else {
            if (EventManager.RopeEvent.OnHandSwitch != null)
                EventManager.RopeEvent.OnHandSwitch();
        }
    }

    private void Update() {
        Animate();
    }

    private void Animate() {
        SetBool("RopeClimbing", _onRope);
        SetBool("AimBow", InputManager.GetKey(InputKey.Aim));

        SetTrigger("FireArrow", InputManager.GetKey(InputKey.Aim) && InputManager.GetKeyDown(InputKey.Shoot));
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