using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("AI/Enemy/SwitchEnemyState")]
    [Help("Plays an animation in the game object")]
    public class SwitchEnemyState : GOAction
    {
        BaseEnemy _enemy;

        [InParam("enemyState")]
        [Help("The enemy state to switch to")]
        public BaseEnemy.EnemyState enemyState;

        public override void OnStart() {
            _enemy = gameObject.GetComponent<BaseEnemy>();
            if (_enemy == null) {
                Debug.LogWarning("Object " + gameObject.name + " is not of type BaseEnemy.");
                return;
            }

            _enemy.SwitchState(enemyState);
        }
    }
}