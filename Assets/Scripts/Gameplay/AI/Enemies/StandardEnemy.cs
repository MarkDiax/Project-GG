using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.AI;

[System.Serializable]
public class EnemyData
{
    public float movementSpeed;
    public float rotationSpeed;
    public float acceleration, deceleration;
    public float targetPrecision;
}

public class StandardEnemy : BaseEnemy
{
    [SerializeField] EnemyData _patrolData;
    [SerializeField] EnemyData _attackData;

    float _currentSpeed;
    int _patrolIndex;
    bool _attacking;

    Coroutine _idleRoutine;

    #region Animation Events

    void A_OnAttackEnd() {
        _attacking = false;
    }

    #endregion

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

        float targetDistance = Vector3.Distance(transform.position, waypoints[_patrolIndex].position);

        if (targetDistance < _patrolData.targetPrecision) {
            _currentSpeed -= _patrolData.deceleration * deltaTime;

            if (_currentSpeed < 0.15f) {
                _currentSpeed = 0f;
                SwitchState(EnemyState.Idle);
                _patrolIndex = GetRandomIndex(_patrolIndex, waypoints.Length);
            }

            return;
        }


        if (!MathX.Float_NearlyEqual(_currentSpeed, _patrolData.movementSpeed, 0.01f)) {

            if (_currentSpeed < _patrolData.movementSpeed)
                _currentSpeed += _patrolData.acceleration * deltaTime;
            else
                _currentSpeed -= _patrolData.deceleration * deltaTime;
        }

        if (_currentSpeed > _patrolData.movementSpeed / 2)
            RotateTowards(waypoints[_patrolIndex].position, _patrolData.rotationSpeed);
    }


    protected override void MoveToAttack() {
        base.MoveToAttack();

        DetectPlayer();

        float targetDistance = Vector3.Distance(transform.position, player.transform.position);

        if (targetDistance < _attackData.targetPrecision) {
            SwitchState(EnemyState.Attack);
            return;
        }

        if (targetDistance > 10)
            SwitchState(EnemyState.Patrol);

        if (_currentSpeed < _attackData.movementSpeed)
            _currentSpeed += _attackData.acceleration * deltaTime;

        if (_currentSpeed > _attackData.movementSpeed / 4)
            RotateTowards(player.transform.position, _attackData.rotationSpeed);
    }

    private void RotateTowards(Vector3 TargetPos, float Speed) {
        Vector3 direction = TargetPos - transform.position;
        direction.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Speed * deltaTime);
    }

    private int GetRandomIndex(int CurrentIndex, int MaxIndex) {
        System.Random rnd = new System.Random();

        int index = rnd.Next(MaxIndex);
        if (index == CurrentIndex)
            return GetRandomIndex(index, MaxIndex);

        return index;
    }


    protected override void Attack() {
        base.Attack();
        if (!_attacking && DetectPlayer()) {
            _attacking = true;
            animator.SetTrigger("Melee1");
            transform.LookAt(player.transform);
            //_currentSpeed = 0f;
        }
        else {
            float targetDistance = Vector3.Distance(transform.position, player.transform.position);

            if (targetDistance > _attackData.targetPrecision) {
                if (DetectPlayer())
                    SwitchState(EnemyState.MoveToAttack);
                else
                    SwitchState(EnemyState.Patrol);
            }
        }
    }

    protected override void Animate() {
        base.Animate();

        animator.SetFloat("MoveDir", _currentSpeed);
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
        if (currentState != EnemyState.MoveToAttack && base.DetectPlayer()) {
            SwitchState(EnemyState.MoveToAttack);
            return true;
        }

        return false;
    }

    public override void TakeDamage(int pDamage) {
        base.TakeDamage(pDamage);

        SwitchState(EnemyState.Attack);
    }
}
