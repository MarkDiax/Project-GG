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
		public float AngleX = 0f, AngleY = 0f;
		public readonly float mouseAngleMinY = 5.0f, mouseAngleMaxY = 80.0f;
		public readonly float sensitivityX = 1.5f, sensitivityY = 1.5f;

		public override void Init() { }

		public void Update() {
			AngleX += Input.GetAxisRaw("Mouse X") * sensitivityX;
			AngleY += Input.GetAxisRaw("Mouse Y") * sensitivityY;
			AngleY = Mathf.Clamp(AngleY, mouseAngleMinY, mouseAngleMaxY);
		}
	}

	public class KeyboardData : Singleton<KeyboardData> {
		public float Vertical, Horizontal;

		public override void Init() { }

		public void Update() {
			Vertical = Input.GetAxisRaw("Vertical");
			Horizontal = Input.GetAxisRaw("Horizontal");
		}
	}
}