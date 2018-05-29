using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

public class InteractableLock : MonoBehaviour
{
    public UnityEvent OnUnlock;

    private bool _locked = true;

    private void Start() {
        OnUnlock.AddListener(() => _locked = false);
    }

    public virtual void Unlock() {
        if (_locked)
            OnUnlock.Invoke(); 
    }

    public bool IsLocked { get { return _locked; } }
}
