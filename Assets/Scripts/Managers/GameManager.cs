using UnityEngine;

/// <summary>
/// Holds all managers such as InputManager.
/// Makes sure the core gameplay features are instantiated.
/// </summary>
public class GameManager : MonoBehaviour {

	InputManager Input = InputManager.Instance;
    void Awake () {
        DontDestroyOnLoad(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
	}

    private void Update()
    {
        UpdateMouseValues();
    }


    private void UpdateMouseValues()
    {
        //_mouseData.mouseAngleX += Input.GetAxis("Mouse X") * _mouseData.sensitivityX;
        //_mouseData.mouseAngleY += Input.GetAxis("Mouse Y") * _mouseData.sensitivityY;
        //_mouseData.mouseAngleY = Mathf.Clamp(_mouseData.mouseAngleY, _mouseData.mouseAngleMinY, _mouseData.mouseAngleMaxY);
    }
}
