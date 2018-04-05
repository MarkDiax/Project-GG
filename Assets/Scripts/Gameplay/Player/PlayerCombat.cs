using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Arrow Attributes")]
    [SerializeField]
    private GameObject _arrow;
    [SerializeField]
    private Transform _arrowSpawnPoint;

    [SerializeField]
    private float _arrowForceAdd, _arrowForceMax;
    private float _arrowForce;

    [Header("Arrow Types")]
    [SerializeField]
    private RopeBehaviour _rope;

    private Player _player;
    private Transform _cam;

    [SerializeField]
    private Text debugtext;


    private void Start() {
        _cam = Camera.main.transform;
        _player = Player.Instance;

        EventManager.PlayerEvent.OnBowDraw.AddListener(OnBowDraw);
        EventManager.PlayerEvent.OnBowShoot += OnBowShoot;
    }

    private void OnBowDraw(bool Drawing) {
        if (!Drawing) {
            _arrowForce = 0;
            return;
        }

        _arrowForce = Mathf.Clamp(_arrowForce + (_arrowForceAdd * Time.deltaTime), 0, _arrowForceMax);

        Debugging();    
    }

    private void Debugging() {

        if (debugtext != null)
            debugtext.text = "DrawCharge: " + _arrowForce;

    }

    private void OnBowShoot() {
        GameObject arrow = Instantiate(_arrow);
        arrow.transform.position = _arrowSpawnPoint.position;
        arrow.transform.rotation = _cam.rotation;


        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(_cam.forward * _arrowForce + Vector3.up * (_arrowForce / 5), ForceMode.Impulse);

        _arrowForce = 0;
    }
}