using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour {
    private Transform _target;
    [SerializeField] private float offset = 10f;

    void Awake() {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) {
            _target = playerController.gameObject.transform;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate() {
        Follow();
        Rotate();
    }

    void Follow() {
        Vector3 targetPosition = _target.position + (offset * _target.up);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
    }

    void Rotate() {
        Quaternion targetRotation = Quaternion.LookRotation(_target.up * -1f, _target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
}
