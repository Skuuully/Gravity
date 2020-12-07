using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    [SerializeField] private Weapon weapon;

    [SerializeField] private float moveSpeed = 5f;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (_playerInput.fire) {
            weapon.Shoot();
        }
    }

    private void FixedUpdate() {
        Vector3 movement = new Vector3(_playerInput.X, 0, _playerInput.Y);
        movement.Normalize();
        if (movement != Vector3.zero) {
            _rigidbody.MovePosition(transform.position + (transform.TransformDirection(movement) * moveSpeed * Time.deltaTime));
        }
    }
}
