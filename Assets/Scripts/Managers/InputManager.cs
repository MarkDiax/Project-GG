using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseData {
	public float mouseAngleX = 0f, mouseAngleY = 0f;
	public float mouseAngleMinY = 5.0f, mouseAngleMaxY = 80.0f;
	public float sensitivityX = 1.5f, sensitivityY = 1.5f;
}

public class InputManager : Singleton<InputManager>{

	public MouseData mouseData;

}
