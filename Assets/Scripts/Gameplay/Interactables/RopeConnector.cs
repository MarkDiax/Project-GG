using UnityEngine;
using System.Collections;

public class RopeConnector : Interactable
{
    [SerializeField]
    private bool _destroyArrowOnImpact;

    [Tooltip("Rope Prefab to spawn on impact")]
    [SerializeField]
    private RopeBehaviour _ropePrefab;

    [Tooltip("Use this if you want the rope to spawn on a particular Transform")]
    [SerializeField]
    private bool _spawnRopeOnPoint;
    [SerializeField]
    private Transform _ropeSpawnPoint;

    public override void Interact(GameObject Object) {
        Vector3 ropeSpawnPosition = Object.transform.position;

        if (_spawnRopeOnPoint)
            ropeSpawnPosition = _ropeSpawnPoint.position;

        if (_destroyArrowOnImpact)
            Destroy(Object);

        Instantiate(_ropePrefab.gameObject, ropeSpawnPosition, Quaternion.identity);
    }
}