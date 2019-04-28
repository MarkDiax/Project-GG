using UnityEngine;

namespace EnemyData
{
    [CreateAssetMenu(fileName = "EnemyPatrolData", menuName = "Config/Enemy/PatrolData")]
    public class PatrolData : BehaviorData
    {
        public float waypointPrecision;
    }
}