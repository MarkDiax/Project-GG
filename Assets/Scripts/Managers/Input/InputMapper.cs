using UnityEngine;
using System.Collections.Generic;

//Do not use!
//This class was an early attempt to make a input mapper that would work with keyboard and controller.
//In coherence with InputManager.cs
/*
[System.Serializable]
public class KeyBind
{
    public string input;
    public InputKey key = InputKey.None;
}

public class InputMapper : MonoSingleton<InputMapper>
{
    [SerializeField]
    private KeyBind[] _inputKeys;

    public Dictionary<int, string> customKeys = new Dictionary<int, string>();

    private void Awake() {
        SaveAllKeys();
    }

    public void SaveAllKeys() {
        customKeys.Clear();
        Save();
    }

    private void Save() {
        for (int i = 0; i < _inputKeys.Length; i++) {
            AddAction(_inputKeys[i].key, _inputKeys[i].input, customKeys);
        }
    }

    private static void AddAction(InputKey Key, string Action, Dictionary<int, string> Collection) {
        if (string.IsNullOrEmpty(Action))
            return;

        if (Collection.ContainsKey((int)Key))
            Collection.Remove((int)Key);

        Collection.Add((int)Key, Action);
    }
}

*/
