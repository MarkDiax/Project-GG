using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePart : MonoBehaviour
{
    private Rigidbody _body;
    private CharacterJoint _joint;
    private CapsuleCollider _collider;

    [HideInInspector]
    public Transform playerHolder;

    private void Awake() {
        _joint = GetComponent<CharacterJoint>();
        _collider = GetComponent<CapsuleCollider>();

        playerHolder = transform.GetChild(0); 
    }

    public float Radius {
        get { return _collider.radius; }
    }

    public CharacterJoint CharacterJoint {
        get { return _joint; }
    }

    public RopeBehaviour Rope { get; set; }

    public int GetRopeIndex() {
        for (int i = 0; i < Rope.ropeSegments.Count; i++) {
            if (this == Rope.ropeSegments[i])
                return i;
        }

        return 0;
    }

    public void IsTrigger(bool Trigger) {
        _collider.isTrigger = Trigger;
    }
}