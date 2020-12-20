using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : MonoBehaviour {
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Quaternion spawnRotation;
    private Walkable _walkable;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;

        var spawnPoint = GetComponentInChildren<PlayerSpawnPoint>().transform;
        spawnPosition = spawnPoint.position;
        spawnRotation = spawnPoint.rotation;

        _walkable = GetComponent<Walkable>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController == null) {
            Debug.LogError("Scene has loaded with a planet but there is no player!");
            return;
        }

        Transform playerTransform = playerController.gameObject.transform; 
        playerTransform.position = spawnPosition;
        playerTransform.rotation = spawnRotation;
        
        if (_walkable != null) {
            _walkable.playerTransform = playerController.gameObject.transform;
        }
    }
}
