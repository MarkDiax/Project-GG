using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class ClimbingController : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed, _acceleration, _deceleration;
    private float _currentSpeed;

    [SerializeField]
    private Transform _leftHand, _rightHand;

    private Player _player;
    private RopePart _currentPart;
    private RopeBehaviour _currentRope;

    private Coroutine _switchHandFocus;

    private float _moveDir;
    private bool _holdRope;

    private void Start() {
        _player = Player.Instance;

        EventManager.RopeEvent.OnHandSwitch += OnHandSwitch;
        EventManager.RopeEvent.OnRopeHold += () => _holdRope = true;
        EventManager.RopeEvent.OnRopeTrigger.AddListener(OnRopeTrigger);
    }

    void OnRopeTrigger(RopePart Part) {
        if (Part == null) {
            DetachFromRope();
            IgnoreCollisionsWithRope(false);
            return;
        }

        AttachToRope(Part);
        IgnoreCollisionsWithRope(true);
    }

    private void OnHandSwitch() {
        if (_currentPart == null)
            return;

        Transform hand = _leftHand.position.y > _rightHand.position.y ? _leftHand : _rightHand;
        if (_moveDir < 1)
            hand = _leftHand.position.y < _rightHand.position.y ? _leftHand : _rightHand;

        float range = float.MaxValue;
        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            float distance = Vector3.Distance(_currentRope.ropeSegments[i].transform.position, hand.position);

            if (distance < range) {
                range = distance;
                _currentPart = _currentRope.ropeSegments[i];
            }
        }

        _holdRope = false;
        _player.transform.SetParent(_currentPart.playerHolder);
    }

    private void Update() {
        if (_currentPart == null)
            return;

        if (InputManager.GetKeyDown(InputKey.Jump)) {
            DetachFromRope();
            _player.transform.position += Vector3.back * 2;
            return;
        }

        _moveDir = InputManager.GetAxis(InputKey.MoveVertical);

        if (_moveDir != 0) {
            Vector3 targetPos = _currentPart.transform.position - Vector3.back * (_currentPart.Radius / 5);
            if (_moveDir < 0)
                targetPos += Vector3.up * _moveDir * 3.4f;

            float animSpeed = Mathf.Abs(_moveDir);

            if (_moveDir != 0) {
                if (_currentSpeed < _maxSpeed) {
                    _currentSpeed += animSpeed * _acceleration;
                }
                else _currentSpeed = _maxSpeed;
            }
            else {
                if (_currentSpeed > 0) {
                    _currentSpeed -= animSpeed * _deceleration;
                }
                else _currentSpeed = 0;
            }

            if (_holdRope)
                targetPos = _player.transform.position;

            _player.transform.position = Vector3.Slerp(_player.transform.position, targetPos, _currentSpeed * Time.deltaTime);
            _player.transform.eulerAngles = new Vector3(0, _player.transform.eulerAngles.y, 0);
        }
        if (EventManager.RopeEvent.OnRopeClimbing != null)
            EventManager.RopeEvent.OnRopeClimbing.Invoke(_moveDir);
    }

    void IgnoreCollisionsWithRope(bool Ignore) {
        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            Physics.IgnoreCollision(GetComponent<CharacterController>(), _currentRope.ropeSegments[i].GetComponent<Collider>(), Ignore);
        }
    }

    void AttachToRope(RopePart Part) {
        _currentPart = Part;
        _currentRope = Part.Rope;

        EventManager.RopeEvent.OnRopeTrigger.RemoveListener(OnRopeTrigger);

        _player.Controller.UseGravity(false);
        _player.Controller.usePhysics = false;
        _player.transform.SetParent(Part.playerHolder);

        if (EventManager.RopeEvent.OnRope != null)
            EventManager.RopeEvent.OnRope.Invoke(true);

        StartCoroutine(MoveToPart(Part));
    }

    private IEnumerator MoveToPart(RopePart Part) {
        while (Vector3.Distance(_player.transform.position, Part.transform.position) > Part.Radius) {
            _player.transform.position = Vector3.Lerp(_player.transform.position, Part.transform.position, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void DetachFromRope() {
        _player.Controller.UseGravity(true);
        _player.Controller.usePhysics = true;
        _player.transform.parent = null;

        if (_currentRope != null) {
            for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
                _currentRope.ropeSegments[i].IsTrigger(false);
            }
        }
        _currentPart = null;
        _currentRope = null;

        EventManager.RopeEvent.OnRopeTrigger.AddListener(OnRopeTrigger);

        if (EventManager.RopeEvent.OnRope != null)
            EventManager.RopeEvent.OnRope.Invoke(false);

    }
}