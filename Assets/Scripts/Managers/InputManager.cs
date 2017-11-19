using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager> {

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

	public class MouseData : Singleton<MouseData> {
		private float angleX = 0f, angleY = 0f;
		private float mouseAngleMinY = 5.0f, mouseAngleMaxY = 80.0f;
		private float sensitivityX = 1.5f, sensitivityY = 1.5f;
		private float scrollWheel;

		public override void Init() { }

		public void Update() {
			angleX += Input.GetAxis("Mouse X") * sensitivityX;
			angleY += Input.GetAxis("Mouse Y") * sensitivityY;
			angleY = Mathf.Clamp(angleY, mouseAngleMinY, mouseAngleMaxY);

			scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		}

		public Vector2 Angle {
			get { return new Vector2(angleX, angleY); }
		}

		public float ScrollWheel {
			get { return scrollWheel; }
		}
	}

	public class KeyboardData : Singleton<KeyboardData> {
		private float vertical, horizontal;

		public override void Init() { }

		public void Update() {
			vertical = Input.GetAxisRaw("Vertical");
			horizontal = Input.GetAxisRaw("Horizontal");
		}

		public float Vertical {
			get { return vertical; }
		}

		public float Horizontal {
			get { return horizontal; }
		}
	}
}