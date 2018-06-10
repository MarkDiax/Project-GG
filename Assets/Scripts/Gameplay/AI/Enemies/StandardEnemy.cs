using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.AI;

public class StandardEnemy : BaseEnemy
{
    [SerializeField] [Header("Movement")] float _maxSpeed;
    [SerializeField] float _acceleration, deceleration;
    [SerializeField] float _rotationSpeed;
    float _velocity, _currentSpeed;
    Transform _target;

    [Tooltip("The range within the AI stops near a waypoint")]
    [SerializeField]
    float _waypointPrecision = 0.5f;
    int _patrolIndex;

    [SerializeField] [Header("Combat")] float _attackRange;

    Coroutine _idleRoutine;

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

        RotateTowards(waypoints[_patrolIndex].position, _rotationSpeed);
        transform.position += transform.forward * (_maxSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, waypoints[_patrolIndex].position) < _waypointPrecision) {
            SwitchState(EnemyState.Idle);
            _patrolIndex = GetRandomIndex(_patrolIndex, waypoints.Length);
            //_target = waypoints[_patrolIndex];
        }
    }

    private void RotateTowards(Vector3 TargetPos, float Speed) {
        Vector3 direction = TargetPos - transform.position;
        direction.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Speed * Time.deltaTime);
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

        RotateTowards(player.transform.position, _rotationSpeed);
        transform.position += transform.forward * (_maxSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.transform.position) <= _attackRange) {
            SwitchState(EnemyState.Attack);
        }
    }

    protected override void Attack() {
        base.Attack();
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
