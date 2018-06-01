using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerCombat : BaseController
{
    [Header("Movement")]
    [SerializeField]
    private float _meleeMoveSpeed;
    [SerializeField]
    private float _walkZoomSpeed;
    [SerializeField]
    private float _walkDrawSpeed;
    private Vector2 _inputDir;
    private Vector3 _moveDir;
    private float _currentSpeed, _targetSpeed;

    [Header("Combat")]
    [SerializeField]
    private BaseWeapon[] _weapons;

    [Header("Arrow Attributes")]
    [SerializeField]
    private GameObject _arrow;
    [SerializeField]
    private Transform _arrowSpawnPoint;
    private float _arrowForce;
    [SerializeField]
    private float _arrowForceAdd, _arrowForceMax, _maxArrowDistance;

    [SerializeField]
    private Text _debugText;

    private bool _drawingBow;
    private bool _inCombat, _currentController;
    private Vector2 _animSpeed;
    private Coroutine _idleTimer = null;
    private bool _eventListening;

    private GameObject _targetedObject;

    protected override void Start() {
        base.Start();

        HandleListeners(true);

        EventManager.PlayerEvent.OnControllerOverride.AddListener((Controller, Uninteruptable) => {
            if (Uninteruptable) {
                HandleListeners(false);
            }
            else if (_eventListening == false)
                HandleListeners(true);
        });
    }

    private void HandleListeners(bool UseListeners) {
        if (UseListeners) {
            EventManager.InputEvent.OnBowDraw.AddListener(OnBowDraw);
            EventManager.InputEvent.OnBowShoot.AddListener(OnBowShoot);
            EventManager.InputEvent.OnMelee.AddListener(OnMelee);
            EventManager.InputEvent.OnCameraZoom.AddListener(OnCameraZoom);
        }
        else {
            EventManager.InputEvent.OnBowDraw.RemoveListener(OnBowDraw);
            EventManager.InputEvent.OnBowShoot.RemoveListener(OnBowShoot);
            EventManager.InputEvent.OnMelee.RemoveListener(OnMelee);
            EventManager.InputEvent.OnCameraZoom.RemoveListener(OnCameraZoom);
        }

        _eventListening = UseListeners;
    }

    protected override void Update() {
        base.Update();

        if (_inCombat && !_currentController) {
            EventManager.PlayerEvent.OnControllerOverride.Invoke(this, false);
            _currentController = true;
        }

        if (InputManager.GetKeyDown(InputKey.Target)) {
            if (_targetedObject != null) {
                _targetedObject = null;
                _inCombat = false;
            }
            else {
                _targetedObject = GetNextTarget();

                if (_targetedObject != null) {
                    ActivateBehaviour();
                    EventManager.PlayerEvent.OnControllerOverride.Invoke(this, false);
                    _currentController = true;
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

    public override void Step() {
        base.Step();
        //TODO change exit incombat
        Debugging();

        //EventManager.AnimationEvent.OnCombatStance.Invoke(_inCombat);

        if (!_inCombat) {
            EventManager.PlayerEvent.OnControllerOverride.Invoke(null, false);
            _currentController = false;
            //DisableWeapons();
        }
    }

    public void A_OnAnimationEnd() {
        if (_targetedObject == null) {
            _targetSpeed = 0f;
            _inCombat = false;
        }
    }

    protected override void Move() {
        _inputDir = new Vector2(InputManager.GetAxis(InputKey.MoveHorizontal), InputManager.GetAxis(InputKey.MoveVertical));
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, 5 * Time.deltaTime);

        Vector2 normalizedInput = new Vector2(_inputDir.x, _inputDir.y).normalized;

        _moveDir.y += gravity * Time.deltaTime;
        _moveDir = new Vector3(normalizedInput.x, 0, normalizedInput.y) * _currentSpeed + Vector3.up * _moveDir.y;
        _moveDir = Quaternion.Euler(0, player.transform.eulerAngles.y, 0) * _moveDir;
        controller.Move(_moveDir * Time.deltaTime);

        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        _animSpeed = Vector2.Lerp(_animSpeed, normalizedInput, 5 * Time.deltaTime);

        if (EventManager.PlayerEvent.OnMove != null)
            EventManager.PlayerEvent.OnMove.Invoke(_animSpeed);
    }

    protected override void Rotate() {
        Quaternion targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, player.transform.eulerAngles.z);

        if (_targetedObject != null) {
            Quaternion lookRotation = Quaternion.LookRotation(_targetedObject.transform.position - player.transform.position);
            targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, lookRotation.eulerAngles.y, player.transform.eulerAngles.z);
        }

        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, 12 + 10 * _moveDir.magnitude * Time.deltaTime);
    }

    private void Debugging() {
        if (_debugText != null)
            _debugText.text = "DrawCharge: " + _arrowForce;
    }

    private void OnMelee() {
        ActivateBehaviour();
        _targetSpeed = 0;
        EventManager.AnimationEvent.UseRootMotion.Invoke(true, true);
        player.Animator.SetTrigger("MeleeLight");
    }

    private void OnBowDraw(bool Drawing) {
        _drawingBow = Drawing;
        _targetSpeed = _walkDrawSpeed;

        if (!_drawingBow) {
            _arrowForce = 0;
            return;
        }

        _arrowForce = Mathf.Clamp(_arrowForce + (_arrowForceAdd * Time.deltaTime), 0, _arrowForceMax);
    }

    private void OnBowShoot() {
        GameObject arrow = Instantiate(_arrow);
        arrow.transform.position = _arrowSpawnPoint.position;

        Vector3 reticlePos = UIManager.Instance.Crosshair.GetComponent<Image>().rectTransform.position;
        Vector3 reticleWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(reticlePos.x, reticlePos.y, _maxArrowDistance));
        Vector3 dir = (reticleWorldPos - _arrowSpawnPoint.position).normalized;

        arrow.GetComponent<Rigidbody>().AddForce(dir * _arrowForce, ForceMode.Impulse);
        arrow.transform.rotation = mainCamera.transform.rotation;
        _arrowForce = 0;
    }

    private void OnCameraZoom(bool Zooming) {
        if (!Zooming) {
            _inCombat = false;
            UIManager.Instance.EnableCrosshair(false);
        }
        else {
            ActivateBehaviour();
            EventManager.AnimationEvent.UseRootMotion.Invoke(false, false);
            _targetSpeed = _walkZoomSpeed;

            if (player.Animator.Animator.GetCurrentAnimatorStateInfo(0).IsTag("AT_Combat_Move"))
                UIManager.Instance.EnableCrosshair(true);
        }
    }

    private void ActivateBehaviour() {
        _inCombat = true;
        //EnableWeapons();

    }

    protected override void UpdateInput() {

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
    }

    //private void EnableWeapons(BaseWeapon Weapon = null) {
    //    if (Weapon != null) {
    //        Weapon.usingWeapon = true;
    //        return;
    //    }

    //    for (int i = 0; i < _weapons.Length; i++)
    //        _weapons[i].usingWeapon = true;
    //}

    //private void DisableWeapons(BaseWeapon Weapon = null) {
    //    if (Weapon != null) {
    //        Weapon.usingWeapon = false;
    //        return;
    //    }

    //    for (int i = 0; i < _weapons.Length; i++)
    //        _weapons[i].usingWeapon = false;
    //}
}