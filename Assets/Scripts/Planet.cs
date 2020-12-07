using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : MonoBehaviour {
    [SerializeField] private Transform spawnPoint;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (spawnPoint == null) {
            spawnPoint = GetComponentInChildren<PlayerSpawnPoint>().transform;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController == null) {
            Debug.LogError("Scene has loaded with a planet but there is no player!");
            return;
        }

        playerController.gameObject.transform.position = spawnPoint.position;
    }
}
