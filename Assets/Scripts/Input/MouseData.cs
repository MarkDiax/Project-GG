using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MouseData
{
    private static MouseData _instance;

    [HideInInspector]
    public float mouseAngleX = 0f, mouseAngleY = 0f;
    public float mouseAngleMinY = 5.0f, mouseAngleMaxY = 80.0f;
    public float sensitivityX = 1.5f, sensitivityY = 1.5f;


    public static MouseData Instance {
        get
        {
            if (_instance == null)
                _instance = new MouseData();
            return _instance;
        }
    }

    private MouseData() { }
}