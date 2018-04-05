using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RopePhysicsObject : Interactable
{
    [SerializeField]
    RopePart _connectedPart;
    RopeBehaviour _connectedRope;

    [SerializeField]
    float _rotationSmoothing;
    float _radius;

    Rigidbody _rigidbody;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _radius = GetComponent<Collider>().bounds.size.y;

        if (_connectedPart != null)
            _connectedRope = _connectedPart.Rope;

        else return;

        EventManager.RopeEvent.OnRopeBreak.AddListener((Rope) => {
            if (Rope == _connectedRope && _connectedPart == null)
                _rigidbody.useGravity = true;
        });
    }

    private void Update() {
        if (_connectedPart == null)
            return;

        transform.position = _connectedPart.transform.position + Vector3.down * _radius;
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, _connectedPart.transform.rotation, _rotationSmoothing * Time.deltaTime);
    }
}
