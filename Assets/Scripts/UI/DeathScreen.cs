using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DeathScreen : UIObject
{
	public UnityEvent OnLoadedDeathscreen;

	[SerializeField] float _appearSpeed = 1f;

	UIImage[] _images;

	protected override void Start() {
		base.Start();

		_images = GetComponentsInChildren<UIImage>(true);

		Reload();

		EventManager.PlayerEvent.OnDeath.AddListener(OnPlayerDeath);
	}

	public void OnPlayerDeath() {
		StartCoroutine(ShowDeathscreen());
	}

	private IEnumerator ShowDeathscreen() {
		List<UIImage> imageList = new List<UIImage>(_images);

		for (int i = 0; i < imageList.Count; i++)
			imageList[i].SetAlpha(0f);

		while (imageList.Count > 0) {
			for (int i = 0; i < imageList.Count; i++) {
				if (imageList[i].GetColor.a >= 1f) {
					imageList.Remove(imageList[i]);
					continue;
				}

				imageList[i].SetAlpha(imageList[i].GetColor.a + _appearSpeed * Time.deltaTime);
			}

			yield return new WaitForEndOfFrame();
		}

		if (OnLoadedDeathscreen != null)
			OnLoadedDeathscreen.Invoke();
	}

	private void Reload() {
		for (int i = 0; i < _images.Length; i++) {
			_images[i].gameObject.SetActive(true);
			_images[i].SetAlpha(0f);
		}
	}

	protected override void OnGameReload() {
		StopAllCoroutines();
		Reload();
	}
}