using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoSingleton<UIManager>
{
	[SerializeField] private GameObject _crosshair;
	[SerializeField] Image _healthBar;

	private float playerHealth;

	private void Start() {
		playerHealth = Player.Instance.Controller.GetHealth;
		EventManager.PlayerEvent.OnHealthChanged.AddListener(UpdateHealthbar);
	}

	public void EnableCrosshair(bool Enable) {
		if (_crosshair.activeSelf != Enable)
			_crosshair.SetActive(Enable);
	}

	private void UpdateHealthbar(float Health) {
		_healthBar.fillAmount = Health / playerHealth;
	}

	public GameObject Crosshair { get { return _crosshair; } }
}