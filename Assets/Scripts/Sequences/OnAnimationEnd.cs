using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class OnAnimationEnd : MonoBehaviour {
    public UnityEvent OnAnimationTrigger;


    public void A_OnAnimationEnd() {
        OnAnimationTrigger.Invoke();
        Debug.Log("Triggered AnimationEvent on: " + name);
    }
}
