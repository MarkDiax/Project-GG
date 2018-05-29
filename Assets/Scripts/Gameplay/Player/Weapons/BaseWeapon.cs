using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField]
    private int _damage;

    protected bool processingCollisions;
    protected List<Collider> activeColliders = new List<Collider>();
    private List<Collider> hasHitDuringAttack = new List<Collider>();

    public bool usingWeapon; // extend so only one weapon is used at a time

    protected virtual void Start() { }

    protected virtual void OnTriggerEnter(Collider Other) {
        if (!activeColliders.Contains(Other))
            activeColliders.Add(Other);
    }

    protected virtual void OnTriggerStay(Collider Other) {
        if (!activeColliders.Contains(Other))
            activeColliders.Add(Other);
    }

    protected virtual void OnTriggerExit(Collider Other) {
        if (activeColliders.Contains(Other)) {
            activeColliders.Remove(Other);
        }
    }

    private void Update() {
        if (activeColliders.Count == 0)
            processingCollisions = false;

        if (processingCollisions) {
            ProcessCollisions();
        }
    }

    protected virtual void ProcessCollisions() {
        for (int i = 0; i < activeColliders.Count; i++) {
            BaseEnemy enemy = activeColliders[i].GetComponent<BaseEnemy>();

            if (enemy != null && !hasHitDuringAttack.Contains(activeColliders[i])) {
                OnEnemyHit(_damage, enemy);
                hasHitDuringAttack.Add(activeColliders[i]);
                activeColliders.Remove(activeColliders[i]);
            }

            InteractableLock iLock = activeColliders[i].GetComponent<InteractableLock>();

            if (iLock != null)
                iLock.Unlock();
        }
    }

    protected void ClearStoredHits() {
        hasHitDuringAttack.Clear();
    }

    protected abstract void OnEnemyHit(int Damage, BaseEnemy Enemy);

    public void Deactivate() {
        usingWeapon = false;
        processingCollisions = false;
        activeColliders.Clear();
    }
    public void Activate() {
        usingWeapon = true;
    }
}