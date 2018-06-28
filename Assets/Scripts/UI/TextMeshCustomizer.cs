using UnityEngine;
using System.Collections;
using TMPro;

public class TextMeshCustomizer : UIObject
{
	[SerializeField] float _revealSpeed;

	TextMeshProUGUI _textMesh;

	protected override void Start() {
		base.Start();

		_textMesh = GetComponent<TextMeshProUGUI>();
		OnGameReload();
	}

	public void ShowOverTime() {
		StartCoroutine(ShowTextMesh(_revealSpeed));
	}

	private IEnumerator ShowTextMesh(float Speed) {
		while (true) {
			_textMesh.alpha += Speed * Time.deltaTime;
			if (_textMesh.alpha >= 1f)
				yield break;

			yield return new WaitForEndOfFrame();
		}
	}

	protected override void OnGameReload() {
		StopAllCoroutines();
		_textMesh.alpha = 0f;
	}
}