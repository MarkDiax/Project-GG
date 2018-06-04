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
    [Header("Default Movement Settings")]
    [SerializeField]
    float _runSpeed = 6;
    float _targetSpeed;

    [SerializeField] float _turnSmoothTime = 0.2f;
    float _turnSmoothVelocity;

    [SerializeField] float _tiltSmoothTime = 0.1f;
    float _tiltSmoothVelocity;

    [SerializeField] float _speedSmoothTime = 0.1f;
    float _speedSmoothVelocity;
    float _currentSpeed;

    [Space]
    [SerializeField] Transform[] _groundCastPoints;
    [SerializeField] float _jumpHeight;
    [SerializeField] float _gravityMod;
    [SerializeField] [Range(0, 1)] float _airControl;
    float _jumpForce;

    Vector2 _inputDir;
    Vector3 _moveDir;
    bool _running;
    #endregion

    #region Combat Fields
    [SerializeField] [Header("Combat Movement Settings")] float _meleeMoveSpeed;
    [SerializeField] float _walkZoomSpeed, _walkDrawSpeed;

    [SerializeField] [Header("Combat Weapons")] Weapon _swordObject;
    [SerializeField] Weapon _bowObject;
    [SerializeField] Transform _bowTransformParent, _swordTransformParent;
    [SerializeField] Transform _bowTransformSheathed, _swordTransformSheathed;
    bool _hasSwordEquipped, _hasBowEquipped;

    [SerializeField] [Header("Other Arrow Properties")] Arrow _arrowPrefab;
    [SerializeField] Transform _arrowSpawnPoint;
    [SerializeField] float _arrowForceAddOnDraw, _arrowForceMax, _arrowDistanceMax;
    float _currentArrowForce;

    bool _drawingBow;
    bool _inCombat;
    Vector2 _animSpeed;
    GameObject _targetedObject;

    #endregion

    bool _isGrounded;
    float _moveDelay = 0f;

    #region Animation Events
    // Animator events don't support boolean parameters, so i'm using ints. 1 = true, 0 = false. 
    void A_OnEquipSword(int Equipped) {
        _hasSwordEquipped = (Equipped == 1);

        if (_hasSwordEquipped) {
            _swordObject.transform.parent = _swordTransformParent;
            _swordObject.transform.localPosition = Vector3.zero;
            _swordObject.transform.localRotation = Quaternion.identity;
        }
        else
            _swordObject.transform.parent = _swordTransformSheathed;

        player.Animator.SetBool("HasSwordEquipped", _hasSwordEquipped);

        if (EventManager.PlayerEvent.OnEquipSword != null)
            EventManager.PlayerEvent.OnEquipSword.Invoke(_hasSwordEquipped);
    }

    // Animator events don't support boolean parameters, so i'm using ints. 1 = true, 0 = false. 
    void A_OnEquipBow(int Equipped) {
        _hasBowEquipped = (Equipped == 1);

        if (_hasBowEquipped) {
            _bowObject.transform.parent = _bowTransformParent;
            _bowObject.transform.localPosition = Vector3.zero;
        }
        else
            _bowObject.transform.parent = _bowTransformSheathed;

        _bowObject.transform.localRotation = Quaternion.identity;
        player.Animator.SetBool("HasBowEquipped", _hasBowEquipped);

        if (EventManager.PlayerEvent.OnEquipBow != null)
            EventManager.PlayerEvent.OnEquipBow.Invoke(_hasBowEquipped);
    }

    // The moment the animation looks like it is about to hit the enemy.
    void A_OnSwordImpact() {
        _swordObject.Attack();
    }

    // Moment when the melee animation ends and the player exits combat.
    void A_OnMeleeEnd() {
        if (_targetedObject == null) {
            _targetSpeed = _runSpeed;
            _inCombat = false;
        }
    }

    // The moment the player starts jumping in the animation.
    void A_OnJump() {
        _jumpForce = Mathf.Sqrt(-2 * gravity * _jumpHeight);
    }

    // Used for suspending movement at Idle Jump or when landing from fall loop.
    void A_OnSuspendMovement(float Delay) {
        _moveDelay = Delay;
    }
    #endregion

    protected override void Awake() {
        base.Awake();

        usePhysics = true;
        gravity = Physics.gravity.y * _gravityMod;
    }

    public override void Resume() {
        HandleEvents(true);
        player.Animator.SetRootMotion(true, false);
        A_OnEquipSword(0);
        A_OnEquipBow(0);
    }

    public override void Suspend() {
        HandleEvents(false);
        player.Animator.SetRootMotion(false, false);
        A_OnEquipSword(0);
        A_OnEquipBow(0);
    }

    private void HandleEvents(bool Active) {
        if (Active) {
            EventManager.InputEvent.OnBowDraw.AddListener(OnBowDraw);
            EventManager.InputEvent.OnCameraZoom.AddListener(OnCameraZoom);
        }
        else {
            EventManager.InputEvent.OnBowDraw.RemoveListener(OnBowDraw);
            EventManager.InputEvent.OnCameraZoom.RemoveListener(OnCameraZoom);
        }
    }

    protected override void UpdateInput() {
        if (_isGrounded) {
            if (InputManager.GetKey(InputKey.Aim) && _hasBowEquipped) {
                if (EventManager.InputEvent.OnCameraZoom != null)
                    EventManager.InputEvent.OnCameraZoom.Invoke(true);

                if (InputManager.GetKey(InputKey.Shoot)) {
                    if (EventManager.InputEvent.OnBowDraw != null)
                        EventManager.InputEvent.OnBowDraw.Invoke(true);
                }
                if (InputManager.GetKeyUp(InputKey.Shoot)) {
                    FireArrow();

                    if (EventManager.InputEvent.OnBowDraw != null)
                        EventManager.InputEvent.OnBowDraw.Invoke(false);
                }
            }
            else if (InputManager.GetKeyDown(InputKey.Melee) && _hasSwordEquipped)
                MeleeAttack();

            if (InputManager.GetKeyUp(InputKey.Aim)) {
                if (EventManager.InputEvent.OnCameraZoom != null)
                    EventManager.InputEvent.OnCameraZoom.Invoke(false);

                if (EventManager.InputEvent.OnBowDraw != null)
                    EventManager.InputEvent.OnBowDraw.Invoke(false);
            }

            if (InputManager.GetKeyDown(InputKey.Target))
                TargetEnemy();

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

            if (InputManager.GetKeyDown(InputKey.Jump) && _isGrounded) {
                player.Animator.SetTrigger("Jump");

            }
        }

        //FOR TESTING ONLY
        if (Input.GetKeyDown(KeyCode.Alpha2))
            player.Animator.SetTrigger("EquipSword");
        if (Input.GetKeyDown(KeyCode.Alpha1))
            player.Animator.SetTrigger("EquipBow");
        //
    }

    private void TargetEnemy() {
        if (InputManager.GetKeyDown(InputKey.Target)) {
            if (_targetedObject != null) {
                _targetedObject = null;
                _inCombat = false;
            }
            else {
                _targetedObject = GetNextTarget();

                if (_targetedObject != null) {
                    _inCombat = true;
                    _targetSpeed = _meleeMoveSpeed;
                }
            }
        }
    }

    private GameObject GetNextTarget() {
        List<BaseEnemy> visibleEnemies = new List<BaseEnemy>();

        for (int i = 0; i < AIManager.Instance.Enemies.Count; i++) {
            List<BaseEnemy> enemies = AIManager.Instance.Enemies;

            Vector3 screenPoint = mainCamera.WorldToViewportPoint(enemies[i].transform.position);
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
                visibleEnemies.Add(enemies[i]);
        }

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < visibleEnemies.Count; i++) {
            float newDistance = Vector3.Distance(visibleEnemies[i].transform.position, player.transform.position);

            if (newDistance < closestDistance) {
                closestEnemy = visibleEnemies[i].gameObject;
                closestDistance = newDistance;
            }
        }

        return closestEnemy;
    }

    private void Move_Default() {
        _targetSpeed = _runSpeed * _inputDir.magnitude;
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_speedSmoothTime));

        if (_moveDelay > 0f)
            _currentSpeed = 0f;

        if (_isGrounded)
            _moveDir.y = 0f;

        if (_jumpForce > 0f) {
            _moveDir.y = _jumpForce;
            _jumpForce = 0f;
        }

        _moveDir = player.transform.forward * _currentSpeed + Vector3.up * _moveDir.y;

    }

    private void Move_Combat() {
        _inputDir = new Vector2(InputManager.GetAxis(InputKey.MoveHorizontal), InputManager.GetAxis(InputKey.MoveVertical));
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, 5 * Time.deltaTime);

        Vector2 normalizedInput = new Vector2(_inputDir.x, _inputDir.y).normalized;

        _moveDir = new Vector3(normalizedInput.x, 0, normalizedInput.y) * _currentSpeed + Vector3.up * _moveDir.y;
        _moveDir = Quaternion.Euler(0, player.transform.eulerAngles.y, 0) * _moveDir;

        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        _animSpeed = Vector2.Lerp(_animSpeed, normalizedInput, 5 * Time.deltaTime);
    }

    public override void Step() {
        if (_jumpForce == 0) 
            _isGrounded = Grounded(); //ground check before the main loop for accurate input
        print("Grounded: " + _isGrounded);

        base.Step();
    }

    protected override void Move() {
        _moveDir.y += gravity * Time.deltaTime;

        if (!_inCombat)
            Move_Default();
        else
            Move_Combat();

        controller.Move(_moveDir * Time.deltaTime);
        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
    }


    private void Rotate_Default() {
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

    private void Rotate_Combat() {
        Quaternion targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, player.transform.eulerAngles.z);

        if (_targetedObject != null) {
            Quaternion lookRotation = Quaternion.LookRotation(_targetedObject.transform.position - player.transform.position);
            targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, lookRotation.eulerAngles.y, player.transform.eulerAngles.z);
        }

        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, 12 + 10 * _moveDir.magnitude * Time.deltaTime);

    }

    protected override void Rotate() {
        if (!_inCombat)
            Rotate_Default();
        else
            Rotate_Combat();
    }

    private void MeleeAttack() {
        _inCombat = true;
        _targetSpeed = 0;
        player.Animator.SetRootMotion(true, true);
        player.Animator.SetTrigger("MeleeLight");
    }

    private void OnBowDraw(bool Drawing) {
        _drawingBow = Drawing;
        _targetSpeed = _walkDrawSpeed;

        if (!_drawingBow) {
            _currentArrowForce = 0;
            return;
        }

        _currentArrowForce = Mathf.Clamp(_currentArrowForce + (_arrowForceAddOnDraw * Time.deltaTime), 0, _arrowForceMax);
    }

    private void FireArrow() {
        player.Animator.SetTrigger("FireArrow");

        GameObject arrow = Instantiate(_arrowPrefab).gameObject;
        arrow.transform.position = _arrowSpawnPoint.position;

        Vector3 reticlePos = UIManager.Instance.Crosshair.GetComponent<Image>().rectTransform.position;
        Vector3 reticleWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(reticlePos.x, reticlePos.y, _arrowDistanceMax));
        Vector3 dir = (reticleWorldPos - _arrowSpawnPoint.position).normalized;

        arrow.GetComponent<Rigidbody>().AddForce(dir * _currentArrowForce, ForceMode.Impulse);
        arrow.transform.rotation = mainCamera.transform.rotation;
        _currentArrowForce = 0;
    }

    private void OnCameraZoom(bool Zooming) {
        if (!Zooming) {
            _inCombat = false;
            UIManager.Instance.EnableCrosshair(false);
        }
        else {
            _inCombat = true;
            _targetSpeed = _walkZoomSpeed;
            player.Animator.SetRootMotion(false, false);

            if (player.Animator.Animator.GetCurrentAnimatorStateInfo(0).IsTag("AT_Combat_Move"))
                UIManager.Instance.EnableCrosshair(true);
        }
    }

    protected override void Animate() {
        float animationSpeed = (_currentSpeed / _runSpeed) * _inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, _speedSmoothTime, Time.deltaTime);
        player.Animator.SetFloat("Move_Combat_X", _animSpeed.x);
        player.Animator.SetFloat("Move_Combat_Y", _animSpeed.y);

        player.Animator.SetBool("Grounded", _isGrounded);
        player.Animator.SetFloat("GroundDistance", DistanceToGround());
        player.Animator.SetBool("InCombat", _inCombat);

        if (_moveDelay > 0)
            _moveDelay -= Time.deltaTime;
    }

    private void LateUpdate() {
        if (!_hasSwordEquipped) {
            _swordObject.transform.localPosition = Vector3.Lerp(_swordObject.transform.localPosition, Vector3.zero, 10 * Time.deltaTime);
            _swordObject.transform.localRotation = Quaternion.Lerp(_swordObject.transform.localRotation, Quaternion.identity, 10 * Time.deltaTime);
        }

        if (!_hasBowEquipped) {
            _bowObject.transform.localPosition = Vector3.Lerp(_bowObject.transform.localPosition, Vector3.zero, 10 * Time.deltaTime);
        }
    }

    private float DistanceToGround() {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, Mathf.Infinity))
            return hitInfo.distance;

        return Mathf.Infinity;
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
        if (_isGrounded)
            return smoothTime;

        if (_airControl == 0)
            return float.MaxValue;

        return smoothTime / _airControl;
    }


    private bool Grounded() {
        if (controller.isGrounded)
            return true;

        RaycastHit hit;

        for (int i = 0; i < _groundCastPoints.Length; i++) {
            if (Physics.Raycast(_groundCastPoints[i].position, Vector3.down, out hit, 0.1f)) {
                controller.Move(Vector3.down * hit.distance);
                return true;
            }
        }

        return false;
    }
}