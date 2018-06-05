using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ButtonPrompt : UIObject
{
    [SerializeField] Image _customImageObject;

    [SerializeField] [Space] string _textMeshText;
    [SerializeField] Sprite _customButtonSprite;

    Image _image;
    Transform _mainCamera;
    TextMeshProUGUI _textMesh;

    private void Start() {
        _mainCamera = Camera.main.transform;
        _image = GetComponent<Image>();
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();

        if (!UsingCustomImage) {
            _textMesh.gameObject.SetActive(true);
            _textMesh.SetText(_textMeshText);
        }

        else {
            _textMesh.gameObject.SetActive(false);
            _customImageObject.gameObject.SetActive(true);
        }

        SetAlpha(0.1f);
    }

    private void Update() {
        transform.LookAt(_mainCamera);
        transform.localRotation *= Quaternion.Euler(Vector3.up * 180);
    }

    public void SetAlpha(float Alpha) {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Alpha);

        if (UsingCustomImage)
            _customImageObject.color = new Color(_customImageObject.color.r, _customImageObject.color.g, _customImageObject.color.b, Alpha);
        else
            _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, Alpha);
    }

    private bool UsingCustomImage {
        get {
            return (_customButtonSprite != null && _customImageObject != null);
        }
    }
}
