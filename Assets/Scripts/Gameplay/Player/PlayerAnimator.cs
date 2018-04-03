using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    private Player _player;
    private Transform _camera;

    private bool _onRope;
    private bool _useRootMotion;
    private bool _drawingBow;

    [Header("Bones")]
    [SerializeField]
    private Transform _spine;

    protected override void Awake() {
        base.Awake();

        _player = Player.Instance;
        _camera = Camera.main.transform;
    }

    private void Start() {
        EventManager.RopeEvent.OnRopeClimbing.AddListener((ClimbSpeed) => SetFloat("climbSpeed", ClimbSpeed));
        EventManager.PlayerEvent.OnBowDraw.AddListener((Drawing) => _drawingBow = Drawing);
        EventManager.RopeEvent.OnRope.AddListener((OnRope) => {
            _onRope = OnRope;
            _useRootMotion = !_onRope;
        });
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

    private void LateUpdate() {
        if (_drawingBow) {

            _player.transform.eulerAngles = new Vector3(_player.transform.eulerAngles.x, _camera.eulerAngles.y, _player.transform.eulerAngles.z);
            _spine.eulerAngles = new Vector3(_spine.eulerAngles.x, _spine.eulerAngles.y, _camera.eulerAngles.x);
        }
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

    public Transform UpperSpine { get { return _spine; } }
}