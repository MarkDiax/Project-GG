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

    public float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float currentSpeed;

    public float jumpHeight;
    public float gravityMod;
    [Range(0, 1)]
    public float airControl;

    [HideInInspector]
    public bool Running, Grounded;

    private Vector2 inputDir;
    private Vector3 moveDir;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        input = InputManager.Instance;
        player = PlayerTracker.Player;
        controller = GetComponent<CharacterController>();
    }

    private void Move()
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = (Running ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        moveDir.y += Physics.gravity.y * gravityMod * Time.deltaTime;
        moveDir = transform.forward * currentSpeed + Vector3.up * moveDir.y;

        controller.Move(moveDir * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
            moveDir.y = 0f;
    }

    private void Update()
    {
        //input
        Vector2 keyboardInput = input.Keyboard.Input;
        inputDir = keyboardInput.normalized;

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            Jump();

        Running = UnityEngine.Input.GetAxisRaw("Run") > 0;

        Move();

        Animate();
    }

    private void Animate()
    {
        player.Animator.SetBool("Grounded", controller.isGrounded);

        float animationSpeed = (Running ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f) * inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, speedSmoothTime, Time.deltaTime);
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            float gravity = Physics.gravity.y * gravityMod;
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