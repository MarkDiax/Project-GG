using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;

public class PlayerController : BaseController
{
    #region Movement Control Fields

    [SerializeField]
    private float _walkSpeed = 2, _runSpeed = 6;

    [SerializeField]
    private float _turnSmoothTime = 0.2f;
    private float _turnSmoothVelocity;

    [SerializeField]
    private float _tiltSmoothTime = 0.1f;
    private float _tiltSmoothVelocity;

    [SerializeField]
    private float _speedSmoothTime = 0.1f;
    private float _speedSmoothVelocity;
    private float _currentSpeed;

    [SerializeField]
    private float _jumpHeight;
    private float _jumpForce;

    private Vector2 _inputDir;
    private Vector3 _moveDir;
    private bool _running;

    [HideInInspector]
    public bool isGrounded;
    #endregion

    private float _moveDelay = 0f;

    public override void Resume() {
        base.Resume();

        HandleEvents(true);

        if (EventManager.AnimationEvent.UseRootMotion != null)
            EventManager.AnimationEvent.UseRootMotion.Invoke(true, false);
    }

    public override void Suspend() {
        base.Suspend();

        HandleEvents(false);

        if (EventManager.AnimationEvent.UseRootMotion != null)
            EventManager.AnimationEvent.UseRootMotion.Invoke(false, false);
    }

    private void HandleEvents(bool Active) {
        if (Active)
            EventManager.AnimationEvent.OnActualJump.AddListener(Jump);
        else
            EventManager.AnimationEvent.OnActualJump.RemoveListener(Jump);
    }

    protected override void UpdateInput() {
        if (isGrounded) {
            if (InputManager.GetKeyDown(InputKey.Melee)) {
                if (EventManager.InputEvent.OnMelee != null)
                    EventManager.InputEvent.OnMelee.Invoke();
            }

            if (InputManager.GetKey(InputKey.Aim)) {
                if (EventManager.InputEvent.OnCameraZoom != null)
                    EventManager.InputEvent.OnCameraZoom.Invoke(true);

                if (InputManager.GetKey(InputKey.Shoot)) {
                    if (EventManager.InputEvent.OnBowDraw != null)
                        EventManager.InputEvent.OnBowDraw.Invoke(true);
                }
                if (InputManager.GetKeyUp(InputKey.Shoot)) {
                    if (EventManager.InputEvent.OnBowShoot != null)
                        EventManager.InputEvent.OnBowShoot.Invoke();

                    if (EventManager.InputEvent.OnBowDraw != null)
                        EventManager.InputEvent.OnBowDraw.Invoke(false);
                }
            }
            if (InputManager.GetKeyUp(InputKey.Aim)) {
                if (EventManager.InputEvent.OnCameraZoom != null)
                    EventManager.InputEvent.OnCameraZoom.Invoke(false);

                if (EventManager.InputEvent.OnBowDraw != null)
                    EventManager.InputEvent.OnBowDraw.Invoke(false);
            }
        }

        if (InputManager.GetKeyDown(InputKey.Interact)) {
            Collider[] objects = Physics.OverlapSphere(player.transform.position, 1.5f);

            for (int i = 0; i < objects.Length; i++) {
                Interactable interactable = objects[i].GetComponent<Interactable>();
                if (interactable != null) {
                    interactable.Interact(gameObject);
                }
            }
        }

        Vector2 keyboardInput = new Vector2(InputManager.GetAxis(InputKey.MoveHorizontal), InputManager.GetAxis(InputKey.MoveVertical));
        _inputDir = keyboardInput.normalized;

        _running = InputManager.GetKey(InputKey.Run);

        if (InputManager.GetKeyDown(InputKey.Jump) && isGrounded) {
            if (EventManager.InputEvent.OnJump != null)
                EventManager.InputEvent.OnJump.Invoke();
        }
    }

    protected override void Move() {
        isGrounded = Grounded();

        //float targetSpeed = (_running ? _runSpeed : _walkSpeed) * _inputDir.magnitude;
        float targetSpeed = _runSpeed * _inputDir.magnitude;//(_running ? runSpeed : walkSpeed) * _inputDir.magnitude;
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_speedSmoothTime));

        if (_moveDelay > 0f)
            _currentSpeed = 0f;

        if (isGrounded)
            _moveDir.y = 0f;

        _moveDir.y += gravity * Time.deltaTime;

        if (_jumpForce > 0f) {
            _moveDir.y = _jumpForce;
            _jumpForce = 0f;
        }

        _moveDir = player.transform.forward * _currentSpeed + Vector3.up * _moveDir.y;

        controller.Move(_moveDir * Time.deltaTime);
        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
    }

    protected override void Rotate() {
        float previousY = player.transform.rotation.eulerAngles.y;

        if (_inputDir != Vector2.zero) {
            //direction
            float targetRotation = Mathf.Atan2(_inputDir.x, _inputDir.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            Vector3 moveVector = Vector3.up * Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(_turnSmoothTime));

            /**
            //z-tilt
            float zOffset = moveVector.y - previousY;
            Vector3 tiltVector = Vector3.forward * Mathf.SmoothDampAngle(player.transform.eulerAngles.z, -zOffset * 1.2f, ref _tiltSmoothVelocity, GetModifiedSmoothTime(_tiltSmoothTime));
          /**/

            //stop rotating when player is falling or is playing land anim
            if (_moveDelay > 0 || DistanceToGround() > 3f)
                moveVector = player.transform.eulerAngles;

            player.transform.eulerAngles = moveVector; //+ tiltVector;
        }
        else {
            float zAxis = Mathf.LerpAngle(player.transform.eulerAngles.z, 0, Time.deltaTime * 50f);
            player.transform.eulerAngles -= Vector3.forward * zAxis;
        }
    }

    protected override void Animate() {
        float animationSpeed = (_currentSpeed / _runSpeed) * _inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, _speedSmoothTime, Time.deltaTime);

        player.Animator.SetBool("Grounded", isGrounded);
        player.Animator.SetFloat("GroundDistance", DistanceToGround());


        if (_moveDelay > 0)
            _moveDelay -= Time.deltaTime;
    }

    private float DistanceToGround() {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, Mathf.Infinity))
            return hitInfo.distance;

        return Mathf.Infinity;
    }

    /// <summary>
    /// Animator event. Used for suspending movement at Idle Jump or when landing from fall loop.
    /// </summary>
    /// <param name="Delay"></param>
    public void A_OnSuspendMovement(float Delay) {
        _moveDelay = Delay;
    }


    private void Jump() {
        _jumpForce = Mathf.Sqrt(-2 * gravity * _jumpHeight);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        //add force to colliding rigidbodies
        AddPhysicsForceOnHit(hit.collider.attachedRigidbody, hit);
    }

    private void AddPhysicsForceOnHit(Rigidbody rigidbody, ControllerColliderHit hit) {
        if (!usePhysics || rigidbody == null || rigidbody.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, hit.moveDirection.z);
        rigidbody.AddForce(pushDir * 50, ForceMode.Force);
    }

    private float GetModifiedSmoothTime(float smoothTime) {
        //if (isGrounded)
        //    return smoothTime;

        if (Config.airControl == 0)
            return float.MaxValue;

        return smoothTime / Config.airControl;
    }


    private bool Grounded() {
        bool rayCheck = Physics.Raycast(player.transform.position, Vector3.down, 0.1f);

        if (controller.isGrounded || rayCheck)
            return true;

        return false;
    }
}