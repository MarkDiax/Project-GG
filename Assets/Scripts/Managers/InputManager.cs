using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//Do not use!
//This class was an early attempt to make a input manager that would work with keyboard and controller.
//In coherence with InputMapper.cs

    /*
public class InputManager : Singleton<InputManager>
{
    public override void Init() { }

    public static bool GetKey(InputKey Key) {
        return Input.GetButton(InputMapper.Instance.customKeys[(int)Key]);
    }

    public static bool GetKeyDown(InputKey Key) {
        return Input.GetButtonDown(InputMapper.Instance.customKeys[(int)Key]);
    }

    public static bool GetKeyUp(InputKey Key) {
        return Input.GetButtonUp(InputMapper.Instance.customKeys[(int)Key]);
    }

    public static float GetAxis(InputKey Key) {
        return Input.GetAxisRaw(InputMapper.Instance.customKeys[(int)Key]);
    }

    public static float GetAxisSmooth(InputKey Key) {
        return Input.GetAxis(InputMapper.Instance.customKeys[(int)Key]);
    }
}
*/