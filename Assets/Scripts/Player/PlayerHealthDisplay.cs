﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayerHealthDisplay : MonoBehaviour {
    [SerializeField] private Health playerHealth;
    private TMP_Text _text;

    private void Awake() {
        _text = GetComponent<TMP_Text>();
        playerHealth.onHealthChangeObservers += OnHealthChange;
    }

    private void Start() {
        _text.text = "HP: " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth;
    }

    private void OnHealthChange() {
        _text.text = "HP: " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth;
    }
}
