using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour {
    [SerializeField] RopePart _startObject;
    [SerializeField] RopePart _particlePrefab;

    [SerializeField] float _segments;
    [SerializeField] float _distanceBetweenSegments;

    List<RopePart> _ropeSegments;

    private void Awake()
    {
        _ropeSegments = new List<RopePart>();
        _ropeSegments.Add(_startObject);

        for (int i = 0; i < _segments; i++)
        {
            RopePart ropePart = Instantiate(_particlePrefab.gameObject, transform).GetComponent<RopePart>();
            _ropeSegments.Add(ropePart);

            ropePart.CharacterJoint.connectedBody = _ropeSegments[i].GetComponent<Rigidbody>();

            ropePart.transform.position = _ropeSegments[i].transform.position + Vector3.down * _distanceBetweenSegments;
            Debug.Log(ropePart.transform.position);
        }
    }
}
