using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using BBUnity.Objects;

namespace BBUnity.Actions
{
    [Action("Navigation/MoveToWaypoint")]
    [Help("Moves the game object to a random waypoint by using a NavMeshAgent")]
    public class MoveToWaypoint : GOAction
    {
        [InParam("Waypoint Object")]
        [Help("Waypoints parent object to use. Should contain all waypoints as childs.")]
        public Waypoint waypointParent;

        private UnityEngine.AI.NavMeshAgent navAgent;

        public override void OnStart() {
            navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent == null) {
                Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
                navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }

            Transform waypoint = GetRandomWaypoint();
            if (waypoint == null)
                Debug.LogWarning("No waypoints have been given for " + gameObject.name + " to navigate to.", gameObject);
            else
                navAgent.SetDestination(waypoint.position);

            navAgent.isStopped = false;
        }

        private Transform GetRandomWaypoint() {
            int Length = waypointParent.waypoints.Length;

            if (Length == 0)
                return null;

            return waypointParent.waypoints[MathX.Int.GetRandomIndex(Length)];
        }

        public override TaskStatus OnUpdate() {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                return TaskStatus.COMPLETED;

            return TaskStatus.RUNNING;
        }

        public override void OnAbort() {
            if (navAgent != null)
                navAgent.isStopped = true;
        }
    }
}
