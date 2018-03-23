using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class ClimbingController : MonoBehaviour
{
    [SerializeField]
    private float _climbSpeed;

    [SerializeField]
    private Transform _leftHand, _rightHand;

    private Player _player;
    private RopePart _currentPart;
    private RopeBehaviour _currentRope;

    public delegate void OnRopeClimbing(float ClimbSpeed);
    public OnRopeClimbing onRopeClimbing;

    public UnityAction OnRopeReleased;

    [HideInInspector]
    public bool OnRope;

    private float _moveDir;
    private int _ropeIndex;

    private void Start() {
        _player = Player.Instance;

        //InputManager.Instance.Keyboard.OnJump += Jump

        _player.Trigger.onRopeTrigger += OnRopeTrigger;
    }

    void OnRopeTrigger(RopePart Part) {
        if (Part == null) {
            DetachFromRope();

            for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
                Physics.IgnoreCollision(GetComponent<CharacterController>(), _currentRope.ropeSegments[i].GetComponent<Collider>(), false);
            }

            return;
        }

        AttachToRope(Part);

        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            Physics.IgnoreCollision(GetComponent<CharacterController>(), _currentRope.ropeSegments[i].GetComponent<Collider>());
        }
    }

    public void Climb() {
        if (_currentPart == null)
            return;

        _ropeIndex = _currentPart.GetRopeIndex();

        _moveDir = InputManager.Instance.GetAxis(InputKey.MoveVertical);

        if (_moveDir != 0) {
            _ropeIndex = Mathf.Clamp(_ropeIndex - Mathf.FloorToInt(_moveDir), 0, _currentRope.ropeSegments.Count - 1);

            if (Vector3.Distance(_player.transform.position, _currentPart.transform.position) < _currentPart.Radius / 2) {

                _currentPart = _currentRope.ropeSegments[_ropeIndex];
                _player.transform.parent = _currentPart.playerHolder;
            }

        }

        // UpdateRopeTriggers();
    }

    private void Update() {
        if (!OnRope || _currentPart == null)
            return;

        Vector3 targetPos = _currentRope.ropeSegments[_ropeIndex].transform.position - Vector3.back * (_currentPart.Radius / 5);
        _player.transform.position = Vector3.Lerp(_player.transform.position, targetPos, _climbSpeed * Time.deltaTime);
        _player.transform.eulerAngles = new Vector3(0, _player.transform.eulerAngles.y, 0);


        Debug.Log(_climbSpeed * _moveDir);
        onRopeClimbing(_climbSpeed * _moveDir);


    }

    private void UpdateRopeTriggers() {
        int depth = 7;

        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            bool trigger = (Mathf.Clamp(i, _ropeIndex - depth, _ropeIndex + depth) == i);
            _currentRope.ropeSegments[i].IsTrigger(trigger);
        }
    }

    void AttachToRope(RopePart Part) {
        _currentPart = Part;
        _currentRope = Part.Rope;
        _ropeIndex = Part.GetRopeIndex();

        _player.Trigger.onRopeTrigger -= OnRopeTrigger;
        _player.Controller.UseGravity(false);
        _player.Controller.usePhysics = false;
        _player.transform.SetParent(Part.playerHolder);

        OnRope = true;
    }

    void DetachFromRope() {
        _player.Controller.UseGravity(true);
        _player.Controller.usePhysics = true;
        _player.transform.parent = null;

        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            _currentRope.ropeSegments[i].IsTrigger(false);
        }
        _currentRope = null;

        _player.Trigger.onRopeTrigger += OnRopeTrigger;

        if (OnRopeReleased != null)
            OnRopeReleased();

        OnRope = false;
    }
}