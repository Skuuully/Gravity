using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour {
    private Transform _spawnPoint;
    [SerializeField] private bool active;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float firstShotTime;

    [SerializeField] private GameObject projectile;

    private void Awake() {
        foreach (Transform child in transform) {
            if (child.gameObject.name == "SpawnPoint") {
                _spawnPoint = child;
                break;
            } 
        }
    }

    private void Start() {
        InvokeRepeating(nameof(Shoot), firstShotTime, fireRate);
    }

    private void Shoot() {
        Instantiate(projectile, _spawnPoint.position, _spawnPoint.rotation);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
