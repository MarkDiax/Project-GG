using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.AI;

public class StandardEnemy : BaseEnemy
{
    private int _patrolIndex;

    [SerializeField] [Tooltip("The range within the AI stops near a waypoint")]
    private float _waypointPrecision = 0.5f;

    [Header("Combat")]
    [SerializeField]
    private float _attackRange;

    private Coroutine _idleRoutine;

    protected override void Start() {
        base.Start();

        SwitchState(EnemyState.Idle);
    }

    protected override void Idle(float TimeInSeconds) {
        base.Idle(TimeInSeconds);

        DetectPlayer();

        if (_idleRoutine == null)
            _idleRoutine = StartCoroutine(IdleTimer(TimeInSeconds));
    }

    protected override void Patrol() {
        base.Patrol();

        DetectPlayer();

        navAgent.destination = waypoints[_patrolIndex].position;
        if (Vector3.Distance(transform.position, waypoints[_patrolIndex].position) < _waypointPrecision) {
            SwitchState(EnemyState.Idle);
            _patrolIndex = GetRandomIndex(_patrolIndex, waypoints.Length);
        }
    }

    private int GetRandomIndex(int CurrentIndex, int MaxIndex) {
        System.Random rnd = new System.Random();

        int index = rnd.Next(MaxIndex);
        if (index == CurrentIndex)
            return GetRandomIndex(index, MaxIndex);

        return index;
    }

    protected override void Move() {
        base.Move();
        navAgent.destination = player.transform.position;

        if (Vector3.Distance(transform.position, player.transform.position) <= _attackRange) {
            SwitchState(EnemyState.Attack);
        }
    }

    protected override void Attack() {
        base.Attack();

        //print("attacking player");
    }


    IEnumerator IdleTimer(float TimeInSeconds) {
        while (TimeInSeconds >= 0) {
            TimeInSeconds -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SwitchState(EnemyState.Patrol);
        _idleRoutine = null;
    }

    protected override bool DetectPlayer() {
        // TODO: CHANGE COLOR BASED ON ALERT STATE!

        if (base.DetectPlayer()) {
            SwitchState(EnemyState.Move);
            return true;
        }

        return false;
    }

    public override void TakeDamage(int pDamage) {
        base.TakeDamage(pDamage);

        SwitchState(EnemyState.Attack);
    }
}
