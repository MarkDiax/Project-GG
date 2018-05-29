using UnityEngine;
using System.Collections;

public class Lever : Interactable
{
    //if the lever can only be pulled once.
    public bool oneShot;

    private Animator _animator;
    private bool _isTriggered;

    private bool _inOriginalPos = true;

    private void Start() {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void Interact(GameObject Object) {
        if (oneShot && _isTriggered)
            return;

        base.Interact(Object);

        _isTriggered = true;
        _inOriginalPos = !_inOriginalPos;
        _animator.SetTrigger("PullLever");
    }

    public bool IsInOriginalPosition {
        get { return _inOriginalPos; }
    }

    public bool InteractOnImpact(Vector3 ObjectPos, Vector3 HitPos) {
        //if arrow hit object at right angle -> Interact

        // if (HitPos.z)
        return false;
    }
}
