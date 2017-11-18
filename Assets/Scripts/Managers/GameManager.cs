using UnityEngine;

public class GameManager : MonoBehaviour {

    MouseData _mouseData = MouseData.Instance;

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
        _mouseData.mouseAngleX += Input.GetAxis("Mouse X") * _mouseData.sensitivityX;
        _mouseData.mouseAngleY += Input.GetAxis("Mouse Y") * _mouseData.sensitivityY;
        _mouseData.mouseAngleY = Mathf.Clamp(_mouseData.mouseAngleY, _mouseData.mouseAngleMinY, _mouseData.mouseAngleMaxY);
    }
}
