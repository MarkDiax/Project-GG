using UnityEngine;
using System.Collections;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected PlayerControllerConfig Config;

    protected Player player;
    protected Camera mainCamera;
    protected InputManager input;
    protected CharacterController controller;

    protected bool usePhysics;
    protected float gravity;

    protected virtual void Awake() {
        player = Player.Instance;
        mainCamera = Camera.main;
        input = InputManager.Instance;
        controller = GetComponent<CharacterController>();

        usePhysics = true;
    }

    protected virtual void Start() {
        UseGravity(true);
    }

    public virtual void Resume() { }
    public virtual void Suspend() { }

    /// <summary>
    /// Runs outside the step loop, is updated regardless if the controller is active or not.
    /// </summary>
    protected virtual void Update() {}

    /// <summary>
    /// Step is updated in Player.cs and is only updated when the controller is active.
    /// </summary>
    public virtual void Step() {
        //print("Step(): " + this);
        UpdateInput();
        Rotate();
        Move();
        Animate();
    }

    protected abstract void UpdateInput();
    protected abstract void Rotate();
    protected abstract void Move();
    protected virtual void Animate() { }

    public void UseGravity(bool Use) {
        gravity = Use ? Physics.gravity.y * Config.gravityMod : 0;
    }
}
