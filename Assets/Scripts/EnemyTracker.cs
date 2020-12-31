using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour {
    public delegate void OnEnemyCountZero();
    
    public event OnEnemyCountZero enemyCountZero;
    
    private int numberOfEnemies = 0;

    public void IncreaseEnemyCount() {
        numberOfEnemies++;
    }

    public void DecreaseEnemyCount() {
        numberOfEnemies--;

        if (numberOfEnemies == 0) {
            enemyCountZero?.Invoke();
        }
    }
}
