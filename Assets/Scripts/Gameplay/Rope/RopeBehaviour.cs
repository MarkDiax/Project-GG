using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour
{
    [SerializeField] RopePart _ropePartPrefab;
    [SerializeField] GameObject _ropeHanger;

    [SerializeField] float _segments;
    [SerializeField] float _distanceBetweenSegments;
    [SerializeField] bool _showColliders;

    [SerializeField] Transform LastChild;

    LineRenderer _line;
    Coroutine _lineRoutine;

    [HideInInspector]
    public List<RopePart> ropeSegments;

    private void Awake() {
        if (ropeSegments != null) {
            for (int i = 0; i < ropeSegments.Count; i++) {
                if (ropeSegments[i] == null) {
                    ropeSegments.Clear();
                    CreateRope();
                    break;
                }
            }
        }
        else
            CreateRope();

        SetupJoints();
        SetupRendering();
    }

    public void CreateRope() {
        ropeSegments = new List<RopePart>();

        for (int i = 0; i < _segments; i++) {
            RopePart ropePart = Instantiate(_ropePartPrefab.gameObject, transform).GetComponent<RopePart>();
            ropeSegments.Add(ropePart);

            ropePart.name = _ropePartPrefab.name + i;
            ropePart.tag = _ropeHanger.tag;
            ropePart.gameObject.layer = _ropeHanger.layer;

            GameObject connectedObject = i > 0 ? ropeSegments[i - 1].gameObject : _ropeHanger;
            ropePart.transform.position = connectedObject.transform.position + (Vector3.down * _distanceBetweenSegments);
        }

        LastChild = ropeSegments[ropeSegments.Count - 1].transform;
    }

    private void SetupJoints() {
        for (int i = 0; i < ropeSegments.Count; i++) {
            ropeSegments[i].GetComponent<Renderer>().enabled = _showColliders;
            ropeSegments[i].Rope = this;

            GameObject connectedObject = i > 0 ? ropeSegments[i - 1].gameObject : _ropeHanger;
            ropeSegments[i].CharacterJoint.connectedBody = connectedObject.GetComponent<Rigidbody>();
        }
    }

    private void SetupRendering() {
        _line = GetComponent<LineRenderer>();

        _line.positionCount = ropeSegments.Count;
        _lineRoutine = StartCoroutine(UpdateLine());
    }

    private IEnumerator UpdateLine() {
        while (true) {

            for (int i = 0; i < _line.positionCount; i++)
                _line.SetPosition(i, ropeSegments[i].transform.position);


            yield return new WaitForFixedUpdate();
        }
    }

    public void Update() {
        if (LastChild == null)
            return;

        float dist = Vector3.Distance(LastChild.position, transform.position);
    }

    public void Respawn() {
        if (EventManager.RopeEvent.OnRopeBreak != null)
            EventManager.RopeEvent.OnRopeBreak.Invoke(this);

        StopCoroutine(_lineRoutine);

        for (int i = 0; i < ropeSegments.Count; i++) {
            Destroy(ropeSegments[i].gameObject);
        }

        ropeSegments = null;
        Awake();
    }
}