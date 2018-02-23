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
    private float turnSmoothVelocity;

    public float tiltSmoothTime = 0.1f;
    private float tiltSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float currentSpeed;

    public float jumpHeight;
    public float gravityMod;
    [Range(0, 1)]
    public float airControl;

    [HideInInspector]
    public bool Running, Grounded, Landed;

    private Vector2 inputDir;
    private Vector3 moveDir;
    private float gravity;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        input = InputManager.Instance;
        player = PlayerTracker.Player;
        controller = GetComponent<CharacterController>();

        gravity = Physics.gravity.y * gravityMod;
    }

    private void Update()
    {
        //input
        Vector2 keyboardInput = input.Keyboard.Input;
        inputDir = keyboardInput.normalized;

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            Jump();

        Running = UnityEngine.Input.GetAxisRaw("Run") > 0;

        Rotate();

        Move();

        Animate();
    }

    private void Move()
    {
        float targetSpeed = (Running ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        moveDir.y += gravity * Time.deltaTime;
        moveDir = transform.forward * currentSpeed + Vector3.up * moveDir.y;

        controller.Move(moveDir * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
            moveDir.y = 0f;
    }

    private void Rotate()
    {
        float previousY = transform.rotation.eulerAngles.y;

        if (inputDir != Vector2.zero)
        {
            //direction
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveVector = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

            //z-tilt
            float zOffset = moveVector.y - previousY;
            Vector3 tiltVector = Vector3.forward * Mathf.SmoothDampAngle(transform.eulerAngles.z, -zOffset * 1.6f, ref tiltSmoothVelocity, GetModifiedSmoothTime(tiltSmoothTime));

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

        float animationSpeed = (Running ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f) * inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, speedSmoothTime, Time.deltaTime);

        player.Animator.SetFloat("VelocityY", controller.velocity.y / gravity);
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            moveDir.y = jumpVelocity;

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

    //private void TiltPlayerWithMouse() {
    //	Vector3 PlayerRot = transform.eulerAngles;
    //	float Tilt = Grounded ? (OldMouseX - input.Mouse.Input.x + PlayerRot.z) : PlayerRot.z;
    //	transform.rotation = Quaternion.Euler(PlayerRot.x, PlayerRot.y, Tilt);
    //	OldMouseX = input.Mouse.Input.x;
    //}
}