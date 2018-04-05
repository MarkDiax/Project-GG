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

    private Coroutine _interactRoutine;
    private Coroutine _climbRoutine;

    private void Start() {
        _player = Player.Instance;

        EventManager.RopeEvent.OnHandSwitch += OnHandSwitch;
        EventManager.RopeEvent.OnRopeHold += () => _holdRope = true;
        EventManager.RopeEvent.OnRopeTrigger.AddListener(OnRopeTrigger);
        EventManager.RopeEvent.OnRopeBreak.AddListener(ReleaseRope);
    }

    void OnRopeTrigger(RopePart Part) {
        if (_interactRoutine != null || _climbRoutine != null)
            return;

        if (Input.GetKeyDown(KeyCode.E) && _interactRoutine == null) {
            _currentRope = Part.Rope;
            IgnoreCollisionsWithRope(true);
            _interactRoutine = StartCoroutine(PullRope());
        }
        else if (Input.GetKeyDown(KeyCode.F) && _climbRoutine == null) {
            AttachToRope(Part);
            IgnoreCollisionsWithRope(true);
            _climbRoutine = StartCoroutine(Climbing());
        }
    }

    private IEnumerator PullRope() {
        RopePart endPoint = _currentRope.ropeSegments[_currentRope.ropeSegments.Count - 1];
        endPoint.transform.parent = _rightHand;
        endPoint.transform.localPosition = Vector3.zero;

        endPoint.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        yield return new WaitForSeconds(0.5f);

        while (true) {
            if (Input.GetKey(KeyCode.E)) {
                endPoint.transform.parent = endPoint.Rope.transform;
                endPoint.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
                ReleaseRope(null);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Climbing() {
        while (true) {
            if (InputManager.GetKey(InputKey.Jump)) {
                ReleaseRope(null);
                DetachFromRope();
                _player.transform.position += Vector3.back * 2;
                yield break;
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
            yield return new WaitForEndOfFrame();
        }
    }

    private void ReleaseRope(RopeBehaviour Rope) {
        if (_interactRoutine != null) {
            StopCoroutine(_interactRoutine);
            IgnoreCollisionsWithRope(false);
            _interactRoutine = null;
        }

        if (_climbRoutine != null) {
            StopCoroutine(_climbRoutine);
            _climbRoutine = null;
            IgnoreCollisionsWithRope(false);
            DetachFromRope();
            _player.transform.position += Vector3.back * 2;
        }
    }

    private void OnHandSwitch() {
        if (_currentPart == null)
            return;

        Transform hand = _leftHand.position.y > _rightHand.position.y ? _leftHand : _rightHand;
        if (_moveDir < 1)
            hand = _leftHand.position.y < _rightHand.position.y ? _leftHand : _rightHand;

        float oldDistance = Vector3.Distance(_currentPart.transform.position, hand.position);

        float range = float.MaxValue;
        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            float distance = Vector3.Distance(_currentRope.ropeSegments[i].transform.position, hand.position);

            if (distance < range) {
                range = distance;
                _currentPart = _currentRope.ropeSegments[i];
            }

            Debug.Log(oldDistance <= distance);
        }

        _holdRope = false;
        _player.transform.SetParent(_currentPart.playerHolder);
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