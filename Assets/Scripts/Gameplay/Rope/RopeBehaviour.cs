using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour
{
    [SerializeField] RopePart _ropePartPrefab;
    [SerializeField] GameObject _ropeHanger;

    public Interactable attachedInteractable;

    [SerializeField] float _segments;
    [SerializeField] float _distanceBetweenSegments;
    [SerializeField] bool _showColliders;

    [SerializeField] Transform LastChild;

    LineRenderer _line;
    Coroutine _lineRoutine;

    [HideInInspector]
    public List<RopePart> ropeSegments;

    private void Awake() {
        SetupRope();
        SetupLine();
    }

    private void SetupRope() {
        ropeSegments = new List<RopePart>();
        for (int i = 0; i < _segments; i++) {
            RopePart ropePart = Instantiate(_ropePartPrefab.gameObject, transform).GetComponent<RopePart>();
            ropeSegments.Add(ropePart);

            ropePart.name = _ropePartPrefab.name + i;
            ropePart.Rope = this;
            ropePart.tag = _ropeHanger.tag;
            ropePart.gameObject.layer = _ropeHanger.layer;
            ropePart.GetComponent<Renderer>().enabled = _showColliders;

            GameObject connectedObject = i > 0 ? ropeSegments[i - 1].gameObject : _ropeHanger;

            ropePart.CharacterJoint.connectedBody = connectedObject.GetComponent<Rigidbody>();
            ropePart.transform.position = connectedObject.transform.position + (Vector3.down * _distanceBetweenSegments);

            LastChild = this.gameObject.transform.GetChild(transform.childCount - 1);
            Debug.Log(LastChild);
        }
    }

    private void SetupLine() {
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

    public void Update()
    {
        float dist = Vector3.Distance(LastChild.position, transform.position);
        //Debug.Log("Distance to other: " + dist);
    }

    public void Respawn() {
        StopCoroutine(_lineRoutine);

        for (int i = 0; i < ropeSegments.Count; i++) {
            Destroy(ropeSegments[i].gameObject);
        }

        ropeSegments.Clear();

        SetupRope();
        SetupLine();
    }
}