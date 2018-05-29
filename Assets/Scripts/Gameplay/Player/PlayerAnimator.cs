using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    private Player _player;
    private Transform _camera;

    private bool _onRope, _drawingBow, _aimingBow;
    private bool[] _useRootMotion = new bool[2];

    [Header("Bones")]
    [SerializeField]
    private Transform _spine;
    private Vector2 _inputDir;

    protected override void Awake() {
        base.Awake();

        _player = Player.Instance;
        _camera = Camera.main.transform;
    }

    private void Start() {
        AddListeners();
    }

    private void AddListeners() {
        EventManager.AnimationEvent.UseRootMotion.AddListener(SetRootMotion);
        EventManager.AnimationEvent.OnCombatStance.AddListener(UpdateCombatStance);

        EventManager.InputEvent.OnJump.AddListener(() => SetTrigger("Jump"));
        EventManager.InputEvent.OnCameraZoom.AddListener(OnZoom);
        EventManager.InputEvent.OnBowDraw.AddListener(OnDrawBow);
        EventManager.InputEvent.OnBowShoot.AddListener(() => SetTrigger("FireArrow"));
        EventManager.PlayerEvent.OnMove.AddListener((Dir) => {
            SetFloat("MoveX", Dir.x);
            SetFloat("MoveY", Dir.y);
            _inputDir = Dir;
        });

        EventManager.RopeEvent.OnRope.AddListener((OnRope) => _onRope = OnRope);
        EventManager.RopeEvent.OnRopeClimbing.AddListener((ClimbSpeed) => SetFloat("ClimbSpeed", ClimbSpeed));
    }

    private void UpdateCombatStance(bool InCombat) {
        SetBool("InCombat", InCombat);
    }

    private void OnDealDamage() {
        if (EventManager.AnimationEvent.OnDealDamage != null)
            EventManager.AnimationEvent.OnDealDamage.Invoke();
    }

    public void OnRopeClimb() {
        if (EventManager.RopeEvent.OnRopeClimb != null)
            EventManager.RopeEvent.OnRopeClimb.Invoke();
    }

    public void OnRopeHold() {
        if (EventManager.RopeEvent.OnRopeHold != null)
            EventManager.RopeEvent.OnRopeHold.Invoke();
    }

    private void OnZoom(bool Zooming) {
        SetBool("AimBow", Zooming);
        _aimingBow = Zooming;
    }

    private void OnDrawBow(bool Drawing) {
        SetBool("DrawBow", Drawing);
        _drawingBow = Drawing;
    }

    public void JumpEvent() {
        if (EventManager.AnimationEvent.OnActualJump != null)
            EventManager.AnimationEvent.OnActualJump.Invoke();
    }

    private void Update() {
        SetBool("RopeClimbing", _onRope);
    }

    private Quaternion _oldSpineRot;
    private void LateUpdate() {
        Quaternion targetSpineRot = _spine.rotation;

        if (_drawingBow)
            targetSpineRot = Quaternion.Euler(_spine.eulerAngles.x, _spine.eulerAngles.y, _camera.eulerAngles.x);

        if (!_aimingBow)
            _oldSpineRot = _spine.rotation;

        _spine.rotation = Quaternion.Lerp(_oldSpineRot, targetSpineRot, 10 * Time.deltaTime);
        _oldSpineRot = _spine.rotation;
    }

    private void OnAnimatorMove() {
        if (_useRootMotion[0])
            transform.position += GetDeltaPosition;
        if (_useRootMotion[1])
            transform.rotation *= GetDeltaRotation;
    }

    private void SetRootMotion(bool UseRootPos, bool UseRootRot) {
        _useRootMotion[0] = UseRootPos;
        _useRootMotion[1] = UseRootRot;
    }

    public Vector3 GetDeltaPosition {
        get { return Animator.deltaPosition; }
    }

    public Quaternion GetDeltaRotation {
        get { return Animator.deltaRotation; }
    }

    public Transform UpperSpine { get { return _spine; } }
}