using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private GameObject _arrow;
    [SerializeField]
    private Transform _arrowSpawnPoint;

    [SerializeField]
    private RopeBehaviour _rope;

    private Player _player;
    private Transform _cam;

    private void Start() {
        _cam = Camera.main.transform;
        _player = Player.Instance;

        EventManager.PlayerEvent.OnBowAim.AddListener(OnBowAim);
        EventManager.PlayerEvent.OnBowShoot += OnBowShoot;
    }

    private LineRenderer _aimRenderer;

    private void OnBowAim(bool Aim) {
        if (!Aim) return;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _cam.eulerAngles.y, _cam.eulerAngles.z);

        if (_aimRenderer == null)
            _aimRenderer = gameObject.AddComponent<LineRenderer>();


        _aimRenderer.positionCount = 2;
        _aimRenderer.SetPosition(0, transform.position + Vector3.up);
        _aimRenderer.SetPosition(1, transform.position + (transform.forward * 2) + Vector3.up);
        _aimRenderer.widthMultiplier = 0.1f;
    }

    private void OnBowShoot() {
        GameObject arrow = Instantiate(_arrow);
        arrow.transform.position = _arrowSpawnPoint.position;

        arrow.transform.rotation = _player.transform.rotation;
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        rb.AddForce((arrow.transform.forward * 5) + (Vector3.up * 2), ForceMode.Impulse);
    }
}