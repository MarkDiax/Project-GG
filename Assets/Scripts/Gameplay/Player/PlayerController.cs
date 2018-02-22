using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private InputManager input;
    private CharacterController character;
    private Transform cameraTransform;

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public bool Running;

    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float currentSpeed;

    public float jumpVelocity;
    [Range(0,1)]
    public float jumpScale;

    public bool Grounded;
    Vector3 moveDirection = Vector3.zero;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        input = InputManager.Instance;
        player = PlayerTracker.Player;
        character = GetComponent<CharacterController>();
    }

    private void Update()
    {
        player.Animator.SetBool("Grounded", Grounded);

        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + new Vector3(0, 0.5f), new Vector3(0,-0.6f));
    }
   
    private void FixedUpdate()
    {
        RaycastHit Hit;
        Grounded = Physics.Raycast(transform.position + new Vector3(0, 0.5f), Vector3.down, out Hit, 0.6f);
    }

    private void Move()
    {
        Vector2 keyboardInput = input.Keyboard.Input;
        Vector2 inputDir = keyboardInput.normalized;

        Running = UnityEngine.Input.GetAxisRaw("Run") > 0;

        if (inputDir != Vector2.zero) {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = (Running ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        float animationSpeed = (Running ? 1 : .5f) * inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, speedSmoothTime, Time.deltaTime);

        moveDirection = transform.forward * currentSpeed + new Vector3(0,moveDirection.y);

        if (character.isGrounded) {

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) {
                player.Animator.SetTrigger("Jump");
                //player.Rigidbody.AddForce(Vector3.up * jumpVelocity);

                moveDirection.y = jumpVelocity;
            }
        }

        moveDirection.y += Physics.gravity.y * jumpScale;
        character.Move(moveDirection * Time.deltaTime);
        //transform.Translate(moveDirection * Time.deltaTime, Space.World);
    }

    //private void TiltPlayerWithMouse() {
    //	Vector3 PlayerRot = transform.eulerAngles;
    //	float Tilt = Grounded ? (OldMouseX - input.Mouse.Input.x + PlayerRot.z) : PlayerRot.z;
    //	transform.rotation = Quaternion.Euler(PlayerRot.x, PlayerRot.y, Tilt);
    //	OldMouseX = input.Mouse.Input.x;
    //}
}