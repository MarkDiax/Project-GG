using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>, IManager
{
    public MouseData Mouse;
    public KeyboardData Keyboard;

    public override void Init() {
        Mouse = MouseData.Instance;
        Keyboard = KeyboardData.Instance;
    }

    public void Update() {
        Mouse.Update();
        Keyboard.Update();
    }

    public class MouseData : Singleton<MouseData>
    {
        private float _yaw = 0f, _pitch = 0f;
        private float _mouseAngleMinY = 0.0f, _mouseAngleMaxY = 90.0f;
        private float _sensitivityX = 1.5f, _sensitivityY = 1.5f;
        private float _scrollWheel;

        public override void Init() { }

        public void Update() {
            if (GameManager.Instance.ControllerConnected) {
                _yaw += UnityEngine.Input.GetAxisRaw("RightStickHorizontal") * _sensitivityX;
                _pitch -= UnityEngine.Input.GetAxisRaw("RightStickVertical") * _sensitivityY;
            }
            else {
                _yaw += UnityEngine.Input.GetAxisRaw("Mouse X") * _sensitivityX;
                _pitch -= UnityEngine.Input.GetAxisRaw("Mouse Y") * _sensitivityY;
            }

            _pitch = Mathf.Clamp(_pitch, _mouseAngleMinY, _mouseAngleMaxY);

            _scrollWheel = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        }

        public Vector2 Input {
            get { return new Vector2(_yaw, _pitch); }
        }

        public float ScrollWheel {
            get { return _scrollWheel; }
        }
    }

    public class KeyboardData : Singleton<KeyboardData>
    {
        private float _vertical, _horizontal;
        private float _jump;

        private bool _hasJumped;

        public UnityAction OnJump;

        public override void Init() {
            OnJump += () => _hasJumped = true;
        }

        public void Update() {
            _vertical = UnityEngine.Input.GetAxisRaw("Vertical");
            _horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
            _jump = UnityEngine.Input.GetAxisRaw("Jump");

            UpdateJumpState();
        }

        private void UpdateJumpState() {
            if (_jump == 1 && !_hasJumped) {
                if (OnJump != null)
                    OnJump();
            }
            else if (_jump == 0)
                _hasJumped = false;
        }

        public Vector2 Input {
            get { return new Vector2(_horizontal, _vertical); }
        }
    }

    public bool GetKey(InputKey Key) {
        return Input.GetButton(InputMapper.Instance.customKeys[(int)Key]);
    }

    public bool GetKeyDown(InputKey Key) {
        return Input.GetButtonDown(InputMapper.Instance.customKeys[(int)Key]);
    }

    public bool GetKeyUp(InputKey Key) {
        return Input.GetButtonUp(InputMapper.Instance.customKeys[(int)Key]);
    }

    public float GetAxis(InputKey Key) {
        return Input.GetAxisRaw(InputMapper.Instance.customKeys[(int)Key]);
    }

    public float GetAxisSmooth(InputKey Key) {
        return Input.GetAxis(InputMapper.Instance.customKeys[(int)Key]);
    }
}