using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePart : MonoBehaviour {

    private Rigidbody _body;

    private CharacterJoint _joint;

    private void Awake()
    {
        _joint = GetComponent<CharacterJoint>();
    }

    public CharacterJoint CharacterJoint {
        get { return _joint; }
    }
}
