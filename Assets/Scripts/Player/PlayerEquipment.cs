using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KnockbackCoverEquipment))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerEquipment : MonoBehaviour {
    private IEquipment _equipment;
    private PlayerInput _playerInput;

    private void Awake() {
        _equipment = GetComponent<KnockbackCoverEquipment>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update() {
        if (_playerInput.rmb) {
            _equipment.Use();
        }
    }
}
