using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedEnemy : MonoBehaviour {
    private EnemyTracker _enemyTracker;

    private void Awake() {
        _enemyTracker = FindObjectOfType<EnemyTracker>();
        if (_enemyTracker == null) {
            Debug.LogWarning(
                "Tracked enemy component but no enemy tracker in scene, will do nothing");
            return;
        }
        _enemyTracker.IncreaseEnemyCount();

        var health = GetComponent<Health>();
        if (health == null) {
            Debug.LogWarning("Has tracked enemy component but no health, how will it ever be killed");
            return;
        }

        health.onDeathObservers += () => { _enemyTracker.DecreaseEnemyCount(); };
    }
}
