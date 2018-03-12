using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour
{
    [SerializeField] RopePart _startObject;
    [SerializeField] RopePart _particlePrefab;

    [SerializeField] float _segments;
    [SerializeField] float _distanceBetweenSegments;

    [SerializeField] bool _showColliders;

    List<RopePart> _ropeSegments;

    LineRenderer _line;

    private void Awake()
    {
        SetupRope();

        SetupLine();
    }

    private void SetupRope()
    {
        _ropeSegments = new List<RopePart>();
        _ropeSegments.Add(_startObject);

        for (int i = 0; i < _segments; i++)
        {
            RopePart ropePart = Instantiate(_particlePrefab.gameObject, transform).GetComponent<RopePart>();
            _ropeSegments.Add(ropePart);

            ropePart.CharacterJoint.connectedBody = _ropeSegments[i].GetComponent<Rigidbody>();

            ropePart.transform.position = _ropeSegments[i].transform.position + Vector3.down * _distanceBetweenSegments;
        }

        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            _ropeSegments[i].GetComponent<Renderer>().enabled = _showColliders;
            _ropeSegments[i].tag = "Rope";
        }
    }

    private void SetupLine()
    {
        _line = GetComponent<LineRenderer>();

        _line.positionCount = _ropeSegments.Count;
    }


    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        for (int i = 0; i < _line.positionCount; i++)
        {
            _line.SetPosition(i, _ropeSegments[i].transform.position);
        }
    }
}