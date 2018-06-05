using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ButtonPrompt : UIObject
{
    [SerializeField] Image _outline;
    [SerializeField] Image _customImageObject;

    [SerializeField] [Space] string _textMeshText;
    [SerializeField] Sprite _customButtonSprite;

    [SerializeField] float _nearDistance, _farDistance;
    [SerializeField] float _nearScale, _farScale;

    Transform _mainCamera;
    Interactable _parent;
    TextMeshProUGUI _textMesh;

    private void Start() {
        _mainCamera = Camera.main.transform;
        _parent = GetComponentInParent<Interactable>();
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _parent.OnInteract.AddListener(DestroySelf);

        if (!UsingCustomImage) {
            _textMesh.gameObject.SetActive(true);
            _textMesh.SetText(_textMeshText);
        }

        else {
            _textMesh.gameObject.SetActive(false);
            _customImageObject.gameObject.SetActive(true);
        }

        //SetAlpha(0f);
    }

    private void DestroySelf() {
        Destroy(gameObject);
    }

    private void Update() {
        transform.LookAt(_mainCamera);
        transform.localRotation *= Quaternion.Euler(Vector3.up * 180);

        float distance = Vector3.Distance(Player.Instance.transform.position, _parent.transform.position);
        float percentToNear = (_nearDistance / distance);

        if (distance < _farDistance) {
            SetAlpha(percentToNear);
            transform.localScale = LerpByPercent(Vector3.one * _farScale, Vector3.one * _nearScale, Mathf.Clamp01(percentToNear));
        }
        else {
            SetAlpha(0f);
        }
    }

    Vector3 LerpByPercent(Vector3 A, Vector3 B, float Percent) {
        return (A + Percent * (B - A));
    }

    public void SetAlpha(float Alpha) {
        _outline.color = new Color(_outline.color.r, _outline.color.g, _outline.color.b, Alpha);

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
