using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoSingleton<UIManager>
{
	[SerializeField] GameObject _crosshair;
	[SerializeField] Image _healthBar;

	private float playerHealth;

	private void Start() {
		playerHealth = Player.Instance.Controller.GetHealth;

		EventManager.PlayerEvent.OnHealthChanged.AddListener(UpdateHealthbar);
		EventManager.GameEvent.OnGameReload.AddListener(OnGameReload);
	}

	private void UpdateHealthbar(float Health) {
		_healthBar.fillAmount = Health / playerHealth;
	}

	public void EnableCrosshair(bool Enable) {
		_crosshair.SetActive(Enable);
	}

	private void OnGameReload() {
		UpdateHealthbar(playerHealth);
		EnableCrosshair(false);
	}

	public GameObject Crosshair { get { return _crosshair; } }
}