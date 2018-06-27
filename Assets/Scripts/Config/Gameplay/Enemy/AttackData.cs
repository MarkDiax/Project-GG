using UnityEngine;

namespace EnemyData
{
    [CreateAssetMenu(fileName = "EnemyAttackData", menuName = "Config/Enemy/AttackData")]
    public class AttackData : BehaviorData
    {
        [Space][Tooltip("The amount of damage the enemy will deal if it is not specified in an animation event.")]
        public int fallbackAttackDamage = 20;
		[Tooltip("The largest distance in which the enemy is still able to attack the player.")]
        public float minimumAttackDistance = 3f;
		[Tooltip("The closest distance the enemy is able to get to the player.")]
		public float maximumAttackDistance = 1.5f;
    }
}