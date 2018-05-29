using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnteredTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;

    [SerializeField]
    [Tooltip("Checks for trigger with tag. If tag is empty, all objects will be checked.")]
    string _checkForTag;

    [SerializeField]
    private bool _oneShot;
    private bool _isTriggered;

    private void OnTriggerEnter(Collider other) {
        if (_oneShot && _isTriggered)
            return;

        if (string.IsNullOrEmpty(_checkForTag) || other.gameObject.CompareTag(_checkForTag)) {
            OnTrigger.Invoke();
            _isTriggered = true;
        }
    }
}