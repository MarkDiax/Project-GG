using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private GameObject _crosshair;

    private void Start() {
        //EventManager.InputEvent.OnCameraZoom.AddListener((Zooming) => EnableCrosshair(Zooming));
    }

    public void EnableCrosshair(bool Enable) {
        if (_crosshair.activeSelf != Enable)
            _crosshair.SetActive(Enable);
    }

    public GameObject Crosshair { get { return _crosshair; } }
}