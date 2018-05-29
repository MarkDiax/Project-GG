using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Holds all managers such as InputManager.
/// Makes sure the core gameplay features are instantiated.
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private bool _lockMouse = true;

    private Coroutine _slowdownRoutine;

    public override void Init() {
        LockMouse(_lockMouse);

        EventManager.Instance.Init();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
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
}