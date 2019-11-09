using UnityEngine;
using System.Collections;

[RequireComponent((typeof(Player)))]
public abstract class BaseController : MonoBehaviour
{
	protected Player player;
	protected Camera mainCamera;
	protected InputManager input;
	protected CharacterController controller;

	protected bool usePhysics;
	protected float gravity;
	protected bool isDead;

	protected virtual void Awake() {
		player = Player.Instance;
		mainCamera = Camera.main;
		controller = GetComponent<CharacterController>();

		usePhysics = true;
	}

	public abstract void Resume();
	public abstract void Suspend();

	/// <summary>
	/// Runs outside the step loop, is updated regardless if the controller is active or not.
	/// </summary>
	protected virtual void Update() { }

	/// <summary>
	/// Step is updated in Player.cs and is only updated when the controller is active.
	/// </summary>
	public virtual void Step() {
		if (isDead)
			return;

		UpdateInput();
		Rotate();
		Move();
		Animate();
	}

	protected abstract void UpdateInput();
	protected abstract void Rotate();
	protected abstract void Move();
	protected virtual void Animate() { }

}
