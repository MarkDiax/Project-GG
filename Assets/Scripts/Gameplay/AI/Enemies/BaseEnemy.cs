using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    private bool _Debugging;

    [SerializeField]
    private int _health;

    [Header("Traversal")]
    [SerializeField]
    protected float idleTime;
    [SerializeField]
    protected Transform[] waypoints;

    [Header("Player Detection")]
    [SerializeField]
    [Tooltip("The range in which the enemy is able to detect the player")]
    private float _lookRange = 10;
    [SerializeField]
    [Tooltip("Angle in which the enemy is able to detect the player")]
    private float _fovCone = 120;

    protected NavMeshAgent navAgent;
    protected Player player;

    protected enum EnemyState
    {
        Idle, Patrol, Move, Attack, Scared, Hide, Taunt, Jump, Climb
    }
    protected EnemyState currentState;

    protected virtual void Awake() {
        AIManager.Instance.Enemies.Add(this);
    }

    protected virtual void OnDestroy() {
        AIManager.Instance.Enemies.Remove(this);
    }
    protected virtual void Start() {
        navAgent = GetComponent<NavMeshAgent>();
        player = Player.Instance;
    }

    protected virtual void Update() {
        switch (currentState) {
            case EnemyState.Idle:
            Idle(idleTime);
            break;
            case EnemyState.Move:
            Move();
            break;
            case EnemyState.Patrol:
            Patrol();
            break;
            case EnemyState.Attack:
            Attack();
            break;
            //etc
        }
    }

    protected virtual void Idle(float TimeInSeconds) { }
    protected virtual void Move() { }
    protected virtual void Patrol() { }
    protected virtual void Attack() { }

    protected void SwitchState(EnemyState NewState) {
        currentState = NewState;

        if (_Debugging)
            print(name + ": switching to AI state " + currentState);
    }

    protected virtual bool DetectPlayer() {
        Vector3 dir = player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir.normalized);

        if (angle <= (_fovCone / 2)) {
            if (Vector3.Distance(transform.position, player.transform.position) <= _lookRange) {
                return true;
            }
        }

        return false;
    }

    public virtual void TakeDamage(int pDamage) {
        _health -= pDamage;

        if (_health <= 0)
            Die();
    }

    public virtual void Die() {
        Destroy(gameObject);
    }
}