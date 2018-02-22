using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : BaseAnimator {

	private void Update() {
		Animate();
	}

	private void Animate() {
        //
	}

    private void OnAnimatorMove()
    {
        transform.position += GetDeltaPosition;
    }

    public void ResetTransform() {
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	public Vector3 GetDeltaPosition {
		get { return Animator.deltaPosition; }
	}

	public Quaternion GetDeltaRotation {
		get { return Animator.deltaRotation; }
	}
}