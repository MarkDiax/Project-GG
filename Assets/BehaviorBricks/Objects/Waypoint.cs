using UnityEngine;

namespace BBUnity.Objects
{
    public class Waypoint : MonoBehaviour
    {
        [HideInInspector]
        public Transform[] waypoints;

        private void Awake() {
            waypoints = new Transform[transform.childCount];

            for (int i = 0; i < waypoints.Length; i++) {
                waypoints[0] = transform.GetChild(0);
            }
        }
    }
}