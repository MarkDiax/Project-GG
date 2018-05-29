using UnityEngine;
using System.Collections.Generic;

public class AIManager : Singleton<AIManager>
{
    public Transform[] Waypoints;
    public List<BaseEnemy> Enemies;

    public override void Init() {
        Enemies = new List<BaseEnemy>();

        GameObject[] objects = GameObject.FindGameObjectsWithTag(Tags.AIWaypoint.ToString());
        Waypoints = new Transform[objects.Length];

        for (int i = 0; i < objects.Length; i++) {
            Waypoints[i] = objects[i].transform;
        }
    }
}