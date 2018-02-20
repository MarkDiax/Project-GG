using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{

    public MouseData Mouse;
    public KeyboardData Keyboard;

    public override void Init()
    {
        Mouse = MouseData.Instance;
        Keyboard = KeyboardData.Instance;
    }

    public void Update()
    {
        Mouse.Update();
        Keyboard.Update();
    }

    public class MouseData : Singleton<MouseData>
    {
        private float yaw = 0f, pitch = 0f;
        private float mouseAngleMinY = 0.0f, mouseAngleMaxY = 90.0f;
        private float sensitivityX = 1.5f, sensitivityY = 1.5f;
        private float scrollWheel;

        public override void Init() { }

        public void Update()
        {
            if (GameManager.Instance.ControllerConnected)
            {
                yaw += UnityEngine.Input.GetAxisRaw("RightStickHorizontal") * sensitivityX;
                pitch -= UnityEngine.Input.GetAxisRaw("RightStickVertical") * sensitivityY;
            }
            else
            {
                yaw += UnityEngine.Input.GetAxisRaw("Mouse X") * sensitivityX;
                pitch -= UnityEngine.Input.GetAxisRaw("Mouse Y") * sensitivityY;
            }

            pitch = Mathf.Clamp(pitch, mouseAngleMinY, mouseAngleMaxY);

            scrollWheel = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        }

        public Vector2 Input {
            get { return new Vector2(yaw, pitch); }
        }

        public float ScrollWheel {
            get { return scrollWheel; }
        }
    }

    public class KeyboardData : Singleton<KeyboardData>
    {
        private float vertical, horizontal;

        public override void Init() { }

        public void Update()
        {
            vertical = UnityEngine.Input.GetAxisRaw("Vertical");
            horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
        }

        public Vector2 Input {
            get { return new Vector2(horizontal, vertical); }
        }
    }
}