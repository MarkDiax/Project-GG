using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Holds all managers such as InputManager.
/// Makes sure the core gameplay features are instantiated.
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField] bool _lockMouse = true;

	Coroutine _slowdownRoutine;
	Cinemachine.CinemachineBrain _cinemachine;

	public override void Init() {
		LockMouse(_lockMouse);

		EventManager.Instance.Init();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			if (EventManager.GameEvent.OnGameReload != null)
				EventManager.GameEvent.OnGameReload.Invoke();

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}

	public void LockMouse(bool Locked) {
		if (Locked) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	public void SlowdownTime(float TimeScale, float Duration) {
		if (_slowdownRoutine != null)
			StopCoroutine(_slowdownRoutine);

		Time.timeScale = TimeScale;
		_slowdownRoutine = StartCoroutine(Internal_SlowdownTime(Duration));
	}

	private IEnumerator Internal_SlowdownTime(float Duration) {
		yield return new WaitForSecondsRealtime(Duration);
		Time.timeScale = 1f;
	}

	public Cinemachine.CinemachineBrain GetCinemachineBrain {
		get {
			if (_cinemachine == null)
				_cinemachine = FindObjectOfType<Cinemachine.CinemachineBrain>();

			return _cinemachine;
		}
	}
}