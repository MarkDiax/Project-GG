using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using BBUnity.Objects;

namespace BBUnity.Actions
{
    [Action("Navigation/GetRandomWaypoint")]
    [Help("Returns a random waypoint.")]
    public class GetRandomWaypoint : GOAction
    {
        [InParam("Waypoint Object")]
        [Help("Waypoints parent object to use. Should contain all waypoints as childs.")]
        public Waypoint waypointParent;

        [OutParam("currentWaypoint")]
        [Help("The returned waypoint")]
        public Transform currentWaypoint;

        public override void OnStart() {
            int Length = waypointParent.waypoints.Length;

            Transform waypoint = waypointParent.waypoints[MathX.Int.GetRandomIndex(Length)];

            if (waypoint == null)
                Debug.LogWarning("No waypoints have been given for " + gameObject.name + " to navigate to.", gameObject);
            else
                currentWaypoint = waypoint;
        }
    }
}