using UnityEngine;

/// <summary>
/// Holds all managers such as InputManager.
/// Makes sure the core gameplay features are instantiated.
/// </summary>
public class GameManager : MonoSingleton<GameManager>, IManager
{
    [SerializeField]
    private bool _lockMouse = true;

    public override void Init() {
        LockMouse(_lockMouse);
    }

    public void Update() {
        InputManager.Instance.Update();
    }

    public bool ControllerConnected {
        get {
            return Input.GetJoystickNames().Length > 0;
        }
    }

    public void LockMouse(bool Locked) {
        if (Locked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}