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
    #endregion

    #region Combat Fields
    [SerializeField] [Header("Combat Movement Settings")] float _meleeMoveSpeed;
    [SerializeField] float _walkZoomSpeed, _walkDrawSpeed;

    [SerializeField] [Header("Combat Weapons")] Transform _bowHolder;
    [SerializeField] Transform _swordHolder;
    [SerializeField] BaseWeapon _swordObject;

    [SerializeField] [Header("Other Arrow Properties")] Arrow _arrowPrefab;
    [SerializeField] Transform _arrowSpawnPoint;
    [SerializeField] float _arrowForceAddOnDraw, _arrowForceMax, _arrowDistanceMax;
    float _currentArrowForce;

    bool _drawingBow;
    bool _inCombat, _currentController;
    Vector2 _animSpeed;
    bool _eventListening;
    GameObject _targetedObject;

    bool _equippedSword, _equippedBow;
    #endregion

    bool _isGrounded;
    float _moveDelay = 0f;

    #region Animation Events
    /// <summary>
    /// Animator events don't support boolean parameters, so i'm using ints. 1 = true, 0 = false. 
    /// </summary>
    /// <param name="Equipped"></param>
    public void A_OnEquipSword(int Equipped) {
        _equippedSword = (Equipped == 1);
    }

    /// <summary>
    /// Animator events don't support boolean parameters, so i'm using ints. 1 = true, 0 = false. 
    /// </summary>
    /// <param name="Equipped"></param>
    public void A_OnEquipBow(int Equipped) {
        _equippedBow = (Equipped == 1);
    }

    /// <summary>
    /// Moment when the melee animation ends and the player exits combat.
    /// </summary>
    public void A_OnMeleeEnd() {
        if (_targetedObject == null) {
            _targetSpeed = _runSpeed;
            _inCombat = false;
        }
    }

    /// <summary>
    /// Used for suspending movement at Idle Jump or when landing from fall loop.
    /// </summary>
    /// <param name="Delay"></param>
    public void A_OnSuspendMovement(float Delay) {
        _moveDelay = Delay;
    }

    /// <summary>
    /// The moment the animation looks like it is about to hit the enemy.
    /// </summary>
    private void A_OnSwordImpact() {
        _swordObject.Attack();
    }
    #endregion

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
        if (Active) {
            EventManager.AnimationEvent.OnActualJump.AddListener(Jump);
            EventManager.InputEvent.OnBowDraw.AddListener(OnBowDraw);
            EventManager.InputEvent.OnBowShoot.AddListener(OnBowShoot);
            EventManager.InputEvent.OnMelee.AddListener(OnMelee);
            EventManager.InputEvent.OnCameraZoom.AddListener(OnCameraZoom);
        }
        else {
            EventManager.AnimationEvent.OnActualJump.RemoveListener(Jump);
            EventManager.InputEvent.OnBowDraw.RemoveListener(OnBowDraw);
            EventManager.InputEvent.OnBowShoot.RemoveListener(OnBowShoot);
            EventManager.InputEvent.OnMelee.RemoveListener(OnMelee);
            EventManager.InputEvent.OnCameraZoom.RemoveListener(OnCameraZoom);
        }
    }

    protected override void UpdateInput() {
        if (_isGrounded) {
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
            else if (InputManager.GetKeyDown(InputKey.Melee)) {
                if (EventManager.InputEvent.OnMelee != null)
                    EventManager.InputEvent.OnMelee.Invoke();
            }

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
                if (EventManager.InputEvent.OnJump != null)
                    EventManager.InputEvent.OnJump.Invoke();
            }

            //FOR TESTING ONLY
            if (Input.GetKeyDown(KeyCode.Alpha2))
                player.Animator.SetTrigger("EquipSword");
            if (Input.GetKeyDown(KeyCode.Alpha1))
                player.Animator.SetTrigger("EquipBow");
            //
        }
    }

    void TargetEnemy() {
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

    GameObject GetNextTarget() {
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

    void DefaultMove() {
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

    void CombatMove() {
        _inputDir = new Vector2(InputManager.GetAxis(InputKey.MoveHorizontal), InputManager.GetAxis(InputKey.MoveVertical));
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, 5 * Time.deltaTime);

        Vector2 normalizedInput = new Vector2(_inputDir.x, _inputDir.y).normalized;

        _moveDir = new Vector3(normalizedInput.x, 0, normalizedInput.y) * _currentSpeed + Vector3.up * _moveDir.y;
        _moveDir = Quaternion.Euler(0, player.transform.eulerAngles.y, 0) * _moveDir;

        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        _animSpeed = Vector2.Lerp(_animSpeed, normalizedInput, 5 * Time.deltaTime);

        if (EventManager.PlayerEvent.OnMove != null)
            EventManager.PlayerEvent.OnMove.Invoke(_animSpeed);
    }

    public override void Step() {
        _isGrounded = Grounded(); //ground check before the main loop for accurate input

        base.Step();
    }

    protected override void Move() {
        _moveDir.y += gravity * Time.deltaTime;

        if (!_inCombat)
            DefaultMove();
        else
            CombatMove();

        controller.Move(_moveDir * Time.deltaTime);
        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
    }


    void DefaultRotate() {
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

    void CombatRotate() {
        Quaternion targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, player.transform.eulerAngles.z);

        if (_targetedObject != null) {
            Quaternion lookRotation = Quaternion.LookRotation(_targetedObject.transform.position - player.transform.position);
            targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, lookRotation.eulerAngles.y, player.transform.eulerAngles.z);
        }

        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, 12 + 10 * _moveDir.magnitude * Time.deltaTime);

    }

    protected override void Rotate() {
        if (!_inCombat)
            DefaultRotate();
        else
            CombatRotate();
    }

    void OnMelee() {
        _inCombat = true;
        _targetSpeed = 0;
        EventManager.AnimationEvent.UseRootMotion.Invoke(true, true);
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

    private void OnBowShoot() {
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
            EventManager.AnimationEvent.UseRootMotion.Invoke(false, false);
            _targetSpeed = _walkZoomSpeed;

            if (player.Animator.Animator.GetCurrentAnimatorStateInfo(0).IsTag("AT_Combat_Move"))
                UIManager.Instance.EnableCrosshair(true);
        }
    }

    protected override void Animate() {
        float animationSpeed = (_currentSpeed / _runSpeed) * _inputDir.magnitude;
        player.Animator.SetFloat("Speed", animationSpeed, _speedSmoothTime, Time.deltaTime);

        player.Animator.SetBool("Grounded", _isGrounded);
        player.Animator.SetFloat("GroundDistance", DistanceToGround());
        player.Animator.SetBool("HasSwordEquipped", _equippedSword);
        player.Animator.SetBool("HasBowEquipped", _equippedBow);
        player.Animator.SetBool("InCombat", _inCombat);

        if (_moveDelay > 0)
            _moveDelay -= Time.deltaTime;
    }

    private float DistanceToGround() {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, Mathf.Infinity))
            return hitInfo.distance;

        return Mathf.Infinity;
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