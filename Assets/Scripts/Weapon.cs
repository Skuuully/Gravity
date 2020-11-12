using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class Weapon : MonoBehaviour {
    [SerializeField] private Transform spawnPoint;
    private Timer _shotTimer;
    [SerializeField] private GameObject _projectilePrefab;

    // Start is called before the first frame update
    void Start() {
        _shotTimer = GetComponent<Timer>();
    }

    public void Shoot() {
        if (_shotTimer.Finished()) {
            _shotTimer.Start();

            Instantiate(_projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
