using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ButtonPrompt : UIObject
{
    [SerializeField] Image _outline;
    [SerializeField] Image _customImageObject;

    [SerializeField] [Space] string _textMeshText;
    [SerializeField] bool _useCustomButton;

    [SerializeField] [Space] float _nearDistance;
    [SerializeField] float _farDistance;
    [SerializeField] float _nearScale, _farScale;

    [SerializeField] [Space] float _disappearTime;

    Transform _mainCamera;
    Transform _parent;
    TextMeshProUGUI _textMesh;
    Coroutine _endingRoutine;

    private void Start() {
        _mainCamera = Camera.main.transform;
        _parent = transform.parent;
        _textMesh = GetComponentInChildren<TextMeshProUGUI>(true);

        Interactable interactable = _parent.GetComponent<Interactable>();
        if (interactable != null)
            interactable.OnInteract.AddListener(OnInteract);

        if (!UsingCustomImage) {
            _textMesh.gameObject.SetActive(true);
            _textMesh.SetText(_textMeshText);
            _customImageObject.gameObject.SetActive(false);
        }

        else {
            _textMesh.gameObject.SetActive(false);
            _customImageObject.gameObject.SetActive(true);
        }
    }

    private void OnInteract() {
        _endingRoutine = StartCoroutine(EndBehaviour());
    }

    private void Update() {
        transform.LookAt(_mainCamera);
        transform.localRotation *= Quaternion.Euler(Vector3.up * 180);

        if (_endingRoutine != null)
            return;

        float distance = Vector3.Distance(Player.Instance.transform.position, _parent.position);
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

    private IEnumerator EndBehaviour() {
        float timer = _disappearTime;

        while (timer > 0) {
            float timePercent = timer / _disappearTime;

            SetAlpha(timePercent);
            transform.localScale = LerpByPercent(Vector3.zero, transform.localScale, timePercent);

            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    private bool UsingCustomImage {
        get {
            return (_useCustomButton && _customImageObject != null);
        }
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
