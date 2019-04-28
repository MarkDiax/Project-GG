using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    [HideInInspector]
    public Animator Animator;

    protected virtual void Awake() {
        Animator = GetComponent<Animator>();
    }

    public void SetTrigger(string Name) {
        Animator.SetTrigger(Name);
    }

    public void SetTrigger(string Name, bool Condition) {
        if (Condition)
            Animator.SetTrigger(Name);
    }

    public void ChangeToState(string State) {
        for (int i = 0; i < Animator.layerCount; i++) {
            if (Animator.GetCurrentAnimatorStateInfo(i).IsName(State))
                return;
        }

        Reset();
        Animator.SetBool(State, true);
    }

    public bool GetBool(string State) {
        return Animator.GetBool(State);
    }

    public void Reset() {
        for (int i = 0; i < Animator.parameters.Length; i++) {

            string Name = Animator.parameters[i].name;
            AnimatorControllerParameterType Type = Animator.parameters[i].type;

            if (Type == AnimatorControllerParameterType.Bool)
                Animator.SetBool(Animator.parameters[i].name, false);
            else if (Type == AnimatorControllerParameterType.Float)
                Animator.SetFloat(Name, 0f);
            else if (Type == AnimatorControllerParameterType.Int)
                Animator.SetInteger(Name, 0);
        }
    }

    public void Reset(string State) {
        for (int i = 0; i < Animator.parameters.Length; i++) {
            if (Animator.parameters[i].name == State.ToString()) {

                string Name = Animator.parameters[i].name;
                AnimatorControllerParameterType Type = Animator.parameters[i].type;

                if (Type == AnimatorControllerParameterType.Bool)
                    Animator.SetBool(Animator.parameters[i].name, false);
                else if (Type == AnimatorControllerParameterType.Float)
                    Animator.SetFloat(Name, 0f);
                else if (Type == AnimatorControllerParameterType.Int)
                    Animator.SetInteger(Name, 0);
            }
        }
    }

    public void SetFloat(string Name, float Value) {
        Animator.SetFloat(Name, Value);
    }

    public void SetFloat(string Name, float Value, float dampTime, float deltaTime) {
        Animator.SetFloat(Name, Value, dampTime, deltaTime);
    }

    public float GetFloat(string Name) {
        return Animator.GetFloat(Name);
    }

    public void SetBool(string Name, bool Value) {
        Animator.SetBool(Name, Value);
    }

    public void SetActive(bool Active) {
        Animator.enabled = Active;
    }

	public void SetInt(string Name, int Value) {
		Animator.SetInteger(Name, Value);
	}
}