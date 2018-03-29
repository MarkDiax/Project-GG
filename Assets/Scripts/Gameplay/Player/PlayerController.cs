using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Player _player;
    private InputManager _input;
    private CharacterController _controller;
    private Transform _cameraTransform;

    #region Movement Control Fields

    public float walkSpeed = 2;
    public float runSpeed = 6;

    public float turnSmoothTime = 0.2f;
    private float _turnSmoothVelocity;

    public float tiltSmoothTime = 0.1f;
    private float _tiltSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    private float _speedSmoothVelocity;
    private float _currentSpeed;

    public float jumpHeight;
    public float gravityMod;
    [Range(0, 1)]
    public float airControl;

    private Vector2 _inputDir;
    private Vector3 _moveDir;
    private float _gravity;
    private bool _running;

    #endregion

    [HideInInspector]
    public bool usePhysics = true;
    private bool _onRope;

    private void Start() {
        _cameraTransform = Camera.main.transform;
        _input = InputManager.Instance;
        _player = Player.Instance;
        _controller = GetComponent<CharacterController>();

        UseGravity(true);

        EventManager.RopeEvent.OnRope.AddListener(OnRope);
    }

    private void Update() {
        UpdateInput();

        if (!_onRope) {
            Rotate();
            Move();
        }

        Animate();
    }

    private void UpdateInput() {
        Vector2 keyboardInput = new Vector2(InputManager.GetAxis(InputKey.MoveHorizontal), InputManager.GetAxis(InputKey.MoveVertical));
        _inputDir = keyboardInput.normalized;

        _running = InputManager.GetKey(InputKey.Run);

        if (InputManager.GetKeyDown(InputKey.Jump)) {
            if (_controller.isGrounded)
                _player.Animator.SetTrigger("Jump");
        }

        if (InputManager.GetKey(InputKey.Aim)) {
            if (EventManager.PlayerEvent.OnBowAim != null)
                EventManager.PlayerEvent.OnBowAim.Invoke(true);

            if (InputManager.GetKeyDown(InputKey.Shoot)) {
                if (EventManager.PlayerEvent.OnBowShoot != null)
                    EventManager.PlayerEvent.OnBowShoot.Invoke();
            }
        }
        if (InputManager.GetKeyUp(InputKey.Aim))
            if (EventManager.PlayerEvent.OnBowAim != null)
                EventManager.PlayerEvent.OnBowAim.Invoke(false);
    }

    private void Move() {
        float targetSpeed = (_running ? runSpeed : walkSpeed) * _inputDir.magnitude;
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        _moveDir.y += _gravity * Time.deltaTime;
        _moveDir = _player.transform.forward * _currentSpeed + Vector3.up * _moveDir.y;

        _controller.Move(_moveDir * Time.deltaTime);
        _currentSpeed = new Vector2(_controller.velocity.x, _controller.velocity.z).magnitude;

        if (_controller.isGrounded)
            _moveDir.y = 0f;
    }

    private void Rotate() {
        float previousY = _player.transform.rotation.eulerAngles.y;

        if (_inputDir != Vector2.zero) {
            //direction
            float targetRotation = Mathf.Atan2(_inputDir.x, _inputDir.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
            Vector3 moveVector = Vector3.up * Mathf.SmoothDampAngle(_player.transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

            //z-tilt
            float zOffset = moveVector.y - previousY;
            Vector3 tiltVector = Vector3.forward * Mathf.SmoothDampAngle(_player.transform.eulerAngles.z, -zOffset * 1.2f, ref _tiltSmoothVelocity, GetModifiedSmoothTime(tiltSmoothTime));

            _player.transform.eulerAngles = moveVector + tiltVector;
        }
        else {
            float zAxis = Mathf.LerpAngle(_player.transform.eulerAngles.z, 0, Time.deltaTime * 50f);
            _player.transform.eulerAngles -= Vector3.forward * zAxis;
        }
    }

    private void Animate() {
        _player.Animator.SetBool("Grounded", _controller.isGrounded);

        float animationSpeed = (_running ? _currentSpeed / runSpeed : _currentSpeed / walkSpeed * 0.5f) * _inputDir.magnitude;
        _player.Animator.SetFloat("Speed", animationSpeed, speedSmoothTime, Time.deltaTime);

        //to fix a floating-point precision issue in the animator:
        float PlayerMoveSpeed = _player.Animator.GetFloat("Speed");
        if (PlayerMoveSpeed < 0.3f && _inputDir.magnitude == 0)
            PlayerMoveSpeed = 0;

        _player.Animator.SetFloat("Speed", PlayerMoveSpeed);
    }

    public void Jump() {
        float jumpVelocity = Mathf.Sqrt(-2 * _gravity * jumpHeight);
        _moveDir.y = jumpVelocity;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        //add force to colliding rigidbodies
        AddPhysicsForceOnHit(hit.collider.attachedRigidbody, hit);
    }

    private float GetModifiedSmoothTime(float smoothTime) {
        if (_controller.isGrounded)
            return smoothTime;

        if (airControl == 0)
            return float.MaxValue;

        return smoothTime / airControl;
    }

    private void AddPhysicsForceOnHit(Rigidbody rigidbody, ControllerColliderHit hit) {
        if (!usePhysics || rigidbody == null || rigidbody.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, hit.moveDirection.z);
        rigidbody.AddForce(pushDir * 100, ForceMode.Force);
    }

    private void OnRope(bool OnRope) {
        _onRope = OnRope;
    }

    public void UseGravity(bool Use) {
        _gravity = Use ? Physics.gravity.y * gravityMod : 0;
    }

    public bool Grounded {
        get { return _controller.isGrounded; }
    }
}