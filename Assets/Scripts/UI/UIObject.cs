using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public abstract class UIObject : MonoBehaviour
{
	protected Canvas canvas;

	protected virtual void Start() {
		canvas = GetComponent<Canvas>();
		EventManager.GameEvent.OnGameReload.AddListener(OnGameReload);
	}

	protected abstract void OnGameReload();
}