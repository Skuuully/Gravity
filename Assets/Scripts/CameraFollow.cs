using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    [SerializeField] private float offset = 10f;

    void Awake() {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) {
            target = playerController.gameObject.transform;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate() {
        if (target != null) {
            Follow();
            Rotate();
        }
    }

    void Follow() {
        Vector3 targetPosition = target.position + (offset * target.up);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
    }

    void Rotate() {
        Quaternion targetRotation = Quaternion.LookRotation(target.up * -1f, target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
}
