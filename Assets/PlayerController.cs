﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;

    [SerializeField] private float moveSpeed = 5f;
    
    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate() {
        Vector3 movement = new Vector3(_playerInput.X, 0, _playerInput.Y);
        movement.Normalize();
        _rigidbody.MovePosition(transform.position + (transform.TransformDirection(movement) * moveSpeed * Time.deltaTime));
    }
}