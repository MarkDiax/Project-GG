using UnityEngine;
using System.Collections.Generic;

public class EnemyWeapon : MonoBehaviour
{
    int _attackDamage;
    bool _processingCollisions;

    List<Collider> _activeColliders = new List<Collider>();
    List<Collider> _hasHitDuringAttack = new List<Collider>();


    private void OnTriggerEnter(Collider Other) {
        if (!_activeColliders.Contains(Other))
            _activeColliders.Add(Other);
    }

    private void OnTriggerStay(Collider Other) {
        if (!_activeColliders.Contains(Other))
            _activeColliders.Add(Other);
    }

    private void OnTriggerExit(Collider Other) {
        if (_activeColliders.Contains(Other)) {
            _activeColliders.Remove(Other);
        }
    }

    private void Update() {
        if (_activeColliders.Count == 0)
            _processingCollisions = false;

        if (_processingCollisions) {
            ProcessCollisions();
        }
    }

    private void ProcessCollisions() {
        for (int i = 0; i < _activeColliders.Count; i++) {
            Player player = _activeColliders[i].GetComponent<Player>();

            if (player != null && !_hasHitDuringAttack.Contains(_activeColliders[i])) {
                OnPlayerHit(_attackDamage, player);

                _hasHitDuringAttack.Add(_activeColliders[i]);
                _activeColliders.Remove(_activeColliders[i]);
                continue;
            }
        }
    }

    private void OnPlayerHit(int Damage, Player Player) {
        print("hit player");
        Player.Controller.TakeDamage(Damage, (Player.transform.position - transform.position).normalized, 0.2f);
    }

    public void Attack(int Damage) {
        _hasHitDuringAttack.Clear();
        _activeColliders.Clear();

        _attackDamage = Damage;
        _processingCollisions = true;
    }
}