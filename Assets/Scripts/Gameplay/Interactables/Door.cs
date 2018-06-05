using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Door : Interactable
{
    private InteractableLock _lock;

    private void Start() {
        _lock = GetComponentInChildren<InteractableLock>();
    }

    public override void Interact(GameObject Object) {
        if (_lock == null || !_lock.IsLocked) {
            base.Interact(Object);

            Destroy(this);
            //destroys door behaviour on unlock.
        }
    }


}