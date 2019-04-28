using UnityEngine;
using System.Collections;

public class Bow : Weapon
{
    public override void Attack() {
        Debug.LogWarning("Bow.Attack() not implemented.");
    }

	public override void EndAttack() {

	}

	protected override void OnEnemyHit(int Damage, BaseEnemy Enemy) {
        Debug.LogWarning("Bow.OnEnemyHit() not implemented.");
    }
}
