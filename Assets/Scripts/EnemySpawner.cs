using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private int maxEnemies;
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnStartTime;
    private int _numEnemiesActive;
    [SerializeField] private GameObject enemyPrefab;
    private Health _health;

    private void Awake() {
        _health = GetComponent<Health>();
        InvokeRepeating(nameof(AttemptSpawnEnemies), spawnStartTime, spawnRate);
    }

    void AttemptSpawnEnemies() {
        if (_numEnemiesActive < maxEnemies) {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            Health enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.onDeathObservers += ReduceActiveCounter;

            if (_health != null) {
                var agent = enemy.GetComponent<Agent>();
                if (agent != null) {
                    agent.AddSafe(_health);
                }
            }
            
            enemy.GetComponent<Agent>().AddSafe(GetComponent<Health>());
            _numEnemiesActive++;
        }
    }

    void ReduceActiveCounter() {
        _numEnemiesActive--;
    }
}
