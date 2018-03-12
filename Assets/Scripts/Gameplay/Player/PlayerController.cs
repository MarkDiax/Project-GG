using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private InputManager input;
    private CharacterController controller;
    private Transform cameraTransform;

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

    [HideInInspector]
    public bool Running, Grounded, Landed;

    private Vector2 _inputDir;
    private Vector3 _moveDir;
    private float _gravity;

    private Coroutine _climbRoutine;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        input = InputManager.Instance;
        player = Player.Instance;
        controller = GetComponent<CharacterController>();

        _gravity = Physics.gravity.y * gravityMod;
    }

    private void Update()
    {
        //input
        Vector2 keyboardInput = input.Keyboard.Input;
        _inputDir = keyboardInput.normalized;

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            Jump();

        Running = UnityEngine.Input.GetAxisRaw("Run") > 0;

        Rotate();

        Move();

        Animate();
    }

    private void Move()
    {
        float targetSpeed = (Running ? runSpeed : walkSpeed) * _inputDir.magnitude;
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        _moveDir.y += _gravity * Time.deltaTime;
        _moveDir = transform.forward * _currentSpeed + Vector3.up * _moveDir.y;

        controller.Move(_moveDir * Time.deltaTime);
        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
            _moveDir.y = 0f;
    }

    private void Rotate()
    {
        float previousY = transform.rotation.eulerAngles.y;

        if (_inputDir != Vector2.zero)
        {
            //direction
            float targetRotation = Mathf.Atan2(_inputDir.x, _inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveVector = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

            //z-tilt
            float zOffset = moveVector.y - previousY;
            Vector3 tiltVector = Vector3.forward * Mathf.SmoothDampAngle(transform.eulerAngles.z, -zOffset * 1.2f, ref _tiltSmoothVelocity, GetModifiedSmoothTime(tiltSmoothTime));

            transform.eulerAngles = moveVector + tiltVector;
        }
        else
        {
            float zAxis = Mathf.LerpAngle(transform.eulerAngles.z, 0, Time.deltaTime * 50f);
            transform.eulerAngles -= Vector3.forward * zAxis;
        }
    }

    private void Animate()
    {
        player.Animator.SetBool("Grounded", controller.isGrounded);

        float animationSpeed = (Running ? _currentSpeed / runSpeed : _currentSpeed / walkSpeed * 0.5f) * _inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, speedSmoothTime, Time.deltaTime);

        //player.Animator.SetFloat("VelocityY", controller.velocity.y / gravity);
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * _gravity * jumpHeight);
            _moveDir.y = jumpVelocity;

            player.Animator.SetTrigger("Jump");
        }
    }

    private float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
            return smoothTime;

        if (airControl == 0)
            return float.MaxValue;

        return smoothTime / airControl;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "Rope")
        {
            Debug.Log("rope");
        }

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, hit.moveDirection.z);
        body.velocity = pushDir * 5f;
    }
}