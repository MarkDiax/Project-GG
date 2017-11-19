using UnityEngine;

/// <summary>
/// Holds all managers such as InputManager.
/// Makes sure the core gameplay features are instantiated.
/// </summary>
public class GameManager : MonoBehaviour {

    void Awake () {
        DontDestroyOnLoad(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
    {
		InputManager.Instance.Update();
    }
}
