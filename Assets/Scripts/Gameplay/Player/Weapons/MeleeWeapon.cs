using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon
{
    [SerializeField] [Header("Slowmotion Effect")] float _slowmoTimescale = 0.1f;
    [SerializeField] float _slowmoDuration = 0.2f;

    public override void Attack() {
        ClearStoredHits();
        processingCollisions = true;
    }

    protected override void OnEnemyHit(int Damage, BaseEnemy Enemy) {
        Enemy.TakeDamage(Damage);
        GameManager.Instance.SlowdownTime(_slowmoTimescale, _slowmoDuration);
    }
}