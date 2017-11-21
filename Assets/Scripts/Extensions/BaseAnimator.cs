using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimator : MonoBehaviour {

	protected Animator Animator;

	protected virtual void Awake() {
		Animator = GetComponent<Animator>();
	}

	public void TriggerExpression(string Parameter) {
		Animator.SetTrigger(Parameter);
	}

	public void ChangeToState(string State) {
		for (int i = 0; i < Animator.layerCount; i++) {
			if (Animator.GetCurrentAnimatorStateInfo(i).IsName(State))
				return;
		}

		Reset();
		Animator.SetBool(State, true);
	}

	public bool CheckBool(string State) {
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

	public void SetActive(bool Active) {
		Animator.enabled = Active;
	}
}