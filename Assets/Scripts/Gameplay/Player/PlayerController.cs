using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private InputManager Input;

    private Transform cameraTransform;

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public bool Running;

    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float currentSpeed;

    public bool Grounded;
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        Input = InputManager.Instance;
        player = PlayerTracker.Player;
    }

    private void Update()
    {
        Move();

        //RaycastGround();

    }

    //private void RaycastGround() {
    //	RaycastHit Info;
    //	Physics.Raycast(new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down), out Info, 0.2f);

    //	Grounded = (Info.collider != null); //&& Info.collider.gameObject.layer == 9);
    //}

    private void Move()
    {
        Vector2 keyboardInput = Input.Keyboard.Input;
        Vector2 inputDir = keyboardInput.normalized;

        if (inputDir != Vector2.zero) {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        Running = UnityEngine.Input.GetAxisRaw("Run") > 0;

        float targetSpeed = (Running ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        float animationSpeed = (Running ? 1 : .5f) * inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, speedSmoothTime, Time.deltaTime);
    }

    public void OnAnimatorMove()
    {
        //apply root motion + physics motion
        transform.position += player.Animator.GetDeltaPosition;
    }

    private void FixedUpdate()
    {
        transform.position += player.Animator.transform.localPosition;

        player.Animator.ResetTransform();
    }


    //private void TiltPlayerWithMouse() {
    //	Vector3 PlayerRot = transform.eulerAngles;
    //	float Tilt = Grounded ? (OldMouseX - input.Mouse.Input.x + PlayerRot.z) : PlayerRot.z;
    //	transform.rotation = Quaternion.Euler(PlayerRot.x, PlayerRot.y, Tilt);
    //	OldMouseX = input.Mouse.Input.x;
    //}
}