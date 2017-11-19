using UnityEngine;

/// <summary>
/// Holds all managers such as InputManager.
/// Makes sure the core gameplay features are instantiated.
/// </summary>
public class GameManager : MonoSingleton<GameManager> {
	
	public override void Init() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update() {
		InputManager.Instance.Update();
	}

	public bool ControllerConnected {
		get {
			return Input.GetJoystickNames().Length > 0;
		}
	}
}