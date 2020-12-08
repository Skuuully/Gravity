using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : MonoBehaviour {
    [SerializeField] private Vector3 spawnPosition;
    private Walkable _walkable;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (spawnPosition == Vector3.zero) {
            spawnPosition = GetComponentInChildren<PlayerSpawnPoint>().transform.position;
        }

        _walkable = GetComponent<Walkable>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController == null) {
            Debug.LogError("Scene has loaded with a planet but there is no player!");
            return;
        }

        playerController.gameObject.transform.position = spawnPosition;
        
        if (_walkable != null) {
            _walkable.playerTransform = playerController.gameObject.transform;
        }
    }
}
