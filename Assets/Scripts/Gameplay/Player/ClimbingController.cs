using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class ClimbingController : BaseController
{
    [SerializeField]
    private float _maxSpeed, _acceleration, _deceleration;
    private float _currentSpeed;
    [SerializeField] [Range(0.1f, 5f)] float _animationSpeedMultiplier = 1f;

    [SerializeField]
    private Transform _leftHand, _rightHand;
    [SerializeField]
    private Transform _rightHandRopePos;

    [SerializeField]
    private float _swingRopeForce;

    private RopePart _currentClimbingNode;
    private RopePart _currentGrabbedNode;
    private RopeBehaviour _currentRope;

    private Coroutine _switchHandFocus;

    private float _moveY;
    private bool _holdRope;

    private Coroutine _interactRoutine;
    private Coroutine _climbRoutine;

    //movement
    private Vector3 _moveDir;
    private Vector2 _inputDir;
    private float _targetSpeed = 3f;

    void Start() {
        //EventManager.RopeEvent.OnRopeTrigger.AddListener(OnRopeTrigger);
        HandleListeners(true);
    }

    public override void Resume() {
        HandleListeners(true);
    }

    public override void Suspend() {
        HandleListeners(false);
    }

    private void HandleListeners(bool Activate) {
        if (Activate) {
            EventManager.RopeEvent.OnRopeClimb.AddListener(OnRopeClimb);
            EventManager.RopeEvent.OnRopeHold.AddListener(() => _holdRope = true);
            EventManager.RopeEvent.OnRopeBreak.AddListener((Part) => OnRopeBreak());
            EventManager.PlayerEvent.OnGrabRope.AddListener(OnGrabRope);

        }
        else {
            EventManager.RopeEvent.OnRopeClimb.RemoveListener(OnRopeClimb);
            EventManager.RopeEvent.OnRopeHold.RemoveListener(() => _holdRope = true);
            EventManager.RopeEvent.OnRopeBreak.RemoveListener((Part) => OnRopeBreak());
        }
    }

    void OnGrabRope(RopePart Part) {
        AttachToRope(Part);
        IgnoreCollisionsWithRope(true);
        _climbRoutine = StartCoroutine(Climbing());
    }

    private void OnRopeBreak() {
        ReleaseRope();
        DetachFromRope();
    }

    protected override void UpdateInput() {
        _inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


    }

    void OnRopeTrigger(RopePart Part) {
        //if (_interactRoutine != null || _climbRoutine != null)
        //    return;

        //if (Input.GetKeyDown(KeyCode.E) && _interactRoutine == null) {
        //    EventManager.PlayerEvent.OnControllerOverride.Invoke(this, false);
        //    _currentRope = Part.Rope;
        //    IgnoreCollisionsWithRope(true);
        //    _interactRoutine = StartCoroutine(PullRope());
        //}
        //else if (Input.GetKeyDown(KeyCode.F) && _climbRoutine == null) {
        //    AttachToRope(Part);
        //    IgnoreCollisionsWithRope(true);
        //    _climbRoutine = StartCoroutine(Climbing());
        //}
    }

    private IEnumerator PullRope() {
        RopePart endPoint = _currentRope.ropeSegments[_currentRope.ropeSegments.Count - 1];
        _currentGrabbedNode = endPoint;
        endPoint.transform.parent = _rightHand;
        endPoint.transform.localPosition = Vector3.zero;

        endPoint.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        yield return new WaitForSeconds(0.5f);

        while (true) {
            if (Input.GetKey(KeyCode.E)) {
                endPoint.transform.parent = endPoint.Rope.transform;
                endPoint.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
                ReleaseRope();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Climbing() {
        if (EventManager.PlayerEvent.OnControllerOverride != null)
            EventManager.PlayerEvent.OnControllerOverride.Invoke(this, true);

        while (true) {

            //if (Physics.Raycast(player.transform.position, -player.transform.up, 2f, (int)Layers.LevelBounds)) {
            //    ReleaseRope();
            //    yield break;
            //}

            if (Input.GetKey(KeyCode.Space)) {
                ReleaseRope();
                yield break;
            }

            _moveY = _inputDir.y;

            _currentClimbingNode.Rigidbody.AddForce(0, -200 * Time.deltaTime, 0, ForceMode.Acceleration);

            if (Input.GetKey(KeyCode.Q)) {
                float _moveX = Input.GetAxis("Horizontal");

                for (int i = _currentClimbingNode.GetRopeIndex(); i > 0; i--) {
                    Vector3 targetDirection = new Vector3(_moveX, 0f, _moveY);
                    targetDirection = mainCamera.transform.TransformDirection(targetDirection);
                    targetDirection.y = 0.0f;

                    float force = _swingRopeForce * (i + _currentRope.ropeSegments.Count / _currentClimbingNode.GetRopeIndex());

                    _currentRope.ropeSegments[i].Rigidbody.AddForce(targetDirection * force * Time.deltaTime, ForceMode.Force);
                }
                _moveY = 0;

                yield return new WaitForEndOfFrame();
            }

            float targetIndex = -_moveY + _currentClimbingNode.GetRopeIndex();
            if (targetIndex < 0 || targetIndex > _currentRope.ropeSegments.Count - 1)
                _moveY = 0;


            //Vector3 r
            //if (_moveY < 0) {
            //    Vector3 target = GetClosestNode(player.transform.position).playerHolder.position;


            //}

            if (_moveY != 0) {
                player.transform.SetParent(GetClosestNode(GetClosestHand().position).playerHolder.transform);

                Vector3 targetPos = _currentClimbingNode.transform.position - Vector3.back * (_currentClimbingNode.Radius / 5);

                if (_moveY < 0) {
                    _currentClimbingNode = GetClosestNode(GetClosestHand().position);
                    targetPos = _currentClimbingNode.transform.position - Vector3.back * (_currentClimbingNode.Radius / 5);

                    //targetPos += Vector3.up * _moveY * 3.4f;
                    targetPos += Vector3.down * 5f;
                }



                float animSpeed = Mathf.Abs(_moveY);

                if (_moveY != 0) {
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
                    targetPos = player.transform.position;

                if (_moveY < 0) {
                    targetPos += Vector3.down;
                }


                _currentSpeed *= 2.5f;

                player.transform.position = Vector3.Slerp(player.transform.position, targetPos, _currentSpeed * Mathf.Abs(_moveY) * Time.deltaTime);
                player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
            }

            player.Animator.SetFloat("Speed", _moveY * _animationSpeedMultiplier);
            yield return new WaitForEndOfFrame();
        }
    }

    private void ReleaseRope() {
        if (_interactRoutine != null) {
            StopCoroutine(_interactRoutine);
            IgnoreCollisionsWithRope(false);
            _currentGrabbedNode = null;
            _interactRoutine = null;
        }

        if (_climbRoutine != null) {
            StopCoroutine(_climbRoutine);
            _climbRoutine = null;
            IgnoreCollisionsWithRope(false);
            DetachFromRope();
        }

        if (EventManager.PlayerEvent.OnControllerOverride != null)
            EventManager.PlayerEvent.OnControllerOverride.Invoke(null, false);
    }

    //updates the currentNode based on hand position
    private void OnRopeClimb() {
        if (_currentClimbingNode == null)
            return;

        _currentClimbingNode = GetClosestNode(GetClosestHand().position);

        _holdRope = false;
    }

    void IgnoreCollisionsWithRope(bool Ignore) {
        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            Physics.IgnoreCollision(GetComponent<CharacterController>(), _currentRope.ropeSegments[i].GetComponent<Collider>(), Ignore);
        }
    }

    void AttachToRope(RopePart Part) {
        //EventManager.RopeEvent.OnRopeTrigger.RemoveListener(OnRopeTrigger);

        _currentClimbingNode = Part;
        _currentRope = Part.Rope;

        usePhysics = false;
        player.transform.SetParent(GetClosestNode(GetClosestHand().position).playerHolder.transform);
        player.Animator.SetBool("RopeClimbing", true);

        StartCoroutine(MoveToPart(Part));
    }

    private IEnumerator MoveToPart(RopePart Part) {
        while (Vector3.Distance(player.transform.position, Part.transform.position) > Part.Radius) {
            player.transform.position = Vector3.Lerp(player.transform.position, Part.transform.position, 5 * Time.deltaTime);
            player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);

            yield return new WaitForEndOfFrame();
        }
    }

    void DetachFromRope() {
        usePhysics = true;
        player.transform.parent = null;
        player.transform.localScale = new Vector3(1, 1, 1);

        if (_currentRope != null) {
            for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
                _currentRope.ropeSegments[i].IsTrigger(false);
            }
        }
        _currentClimbingNode = null;
        _currentRope = null;

        //EventManager.RopeEvent.OnRopeTrigger.AddListener(OnRopeTrigger);

        player.Animator.SetBool("RopeClimbing", false);
    }

    protected override void Rotate() {
        if (_climbRoutine != null)
            return;

        Quaternion targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, player.transform.eulerAngles.z);

        //if (_currentGrabbedNode != null) {
        //    Quaternion lookRotation = Quaternion.LookRotation(_currentGrabbedNode.transform.position - player.transform.position);
        //    targetRotation = Quaternion.Euler(player.transform.eulerAngles.x, lookRotation.eulerAngles.y, player.transform.eulerAngles.z);
        //}

        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, 12 + 10 * _moveDir.magnitude * Time.deltaTime);
    }


    private void OnDrawGizmos() {

        //Gizmos.DrawLine(player.transform.position, player.transform.position + -transform.up * 2f);

        //if (_currentGrabbedNode != null) {

        //    int index = Mathf.FloorToInt(_currentRope.ropeSegments.Count / 2);
        //    Gizmos.DrawSphere(_currentRope.ropeSegments[index].transform.position, 2);
        //}
    }


    private void LateUpdate() {
        if (_currentGrabbedNode != null)
            _rightHand.parent.position = _rightHandRopePos.position;
    }

    protected override void Move() {
        if (_climbRoutine != null)
            return;

        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, 5 * Time.deltaTime);

        Vector2 normalizedInput = new Vector2(_inputDir.x, _inputDir.y).normalized;

        _moveDir.y += gravity * Time.deltaTime;
        _moveDir = new Vector3(normalizedInput.x, 0, normalizedInput.y) * _targetSpeed + Vector3.up * _moveDir.y;
        _moveDir = Quaternion.Euler(0, player.transform.eulerAngles.y, 0) * _moveDir;

        if (_currentGrabbedNode != null) {
            int index = Mathf.FloorToInt(_currentRope.ropeSegments.Count / 2);
            Collider[] sphereCollider = Physics.OverlapSphere(_currentRope.ropeSegments[index].transform.position, 2);

            float lockDistance = 4f;

            bool inRadius = false;

            for (int i = 0; i < sphereCollider.Length; i++) {
                if (sphereCollider[i] == player.GetComponent<Collider>()) {
                    inRadius = true;
                    break;
                }
            }

            if (!inRadius) {
                float distanceBetweenNode = Vector3.Distance(_currentRope.ropeSegments[index].transform.position, player.transform.position);
                float nextCalculatedDistance = Vector3.Distance(player.transform.position + _moveDir * Time.deltaTime, _currentRope.ropeSegments[index].transform.position);

                //if (distanceBetweenNode > nextCalculatedDistance)
                //    print("moving forward");

                if (distanceBetweenNode < nextCalculatedDistance) {
                    _moveDir /= distanceBetweenNode / 2;

                    if (distanceBetweenNode >= lockDistance) {
                        _moveDir = Vector3.zero;
                    }
                }
            }
        }

        controller.Move(_moveDir * Time.deltaTime);

        _currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
    }

    private RopePart GetClosestNode(Vector3 ToPosition) {
        float range = float.MaxValue;
        RopePart closestNode = _currentClimbingNode;

        for (int i = 0; i < _currentRope.ropeSegments.Count; i++) {
            float distance = Vector3.Distance(_currentRope.ropeSegments[i].transform.position, ToPosition);

            if (distance < range) {
                range = distance;
                closestNode = _currentRope.ropeSegments[i];
            }
        }

        return closestNode;
    }

    private Transform GetClosestHand() {
        Transform closestHand = _leftHand.position.y > _rightHand.position.y ? _leftHand : _rightHand;
        if (_moveY < 1)
            closestHand = _leftHand.position.y < _rightHand.position.y ? _leftHand : _rightHand;

        return closestHand;
    }
}