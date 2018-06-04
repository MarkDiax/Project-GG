using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public class UIObject : MonoBehaviour
{
    protected Canvas canvas;

    protected virtual void Awake() {
        canvas = GetComponent<Canvas>();
    }
}
