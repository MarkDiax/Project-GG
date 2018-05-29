using UnityEngine;
using System.Collections;

public class MeleeWeapon : BaseWeapon
{
    [Header("Slowmotion Effect")]
    [SerializeField]
    private float _slowmoTimescale = 0.1f;
    [SerializeField]
    private float _slowmoDuration = 0.2f;

    protected override void Start() {
        base.Start();

        //start looking for collisions on melee animation event
        EventManager.AnimationEvent.OnDealDamage.AddListener(() => {
            if (usingWeapon) {
                ClearStoredHits();
                processingCollisions = true;
            }
        });
    }

    protected override void OnEnemyHit(int Damage, BaseEnemy Enemy) {
        Enemy.TakeDamage(Damage);
        GameManager.Instance.SlowdownTime(_slowmoTimescale, _slowmoDuration);
    }
}