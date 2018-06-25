using UnityEngine;

namespace EnemyData
{
    [CreateAssetMenu(fileName = "EnemyAttackData", menuName = "Config/Enemy/AttackData")]
    public class AttackData : BehaviorData
    {
        [Space][Tooltip("The amount of damage the enemy will deal if it is not specified in an animation event.")]
        public int fallbackAttackDamage = 20;
        public float minimumAttackDistance = 3f;
    }
}