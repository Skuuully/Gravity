using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MaxHealthInteractable : MonoBehaviour, IInteractable {
    [SerializeField] private int cost = 10;
    [SerializeField] private int maxHealthIncrease = 1;
    private string _displayText = "";
    private Health _playerHealth;

    private void Awake() {
        _displayText = "Costs - " + cost + ", increases max health by " + maxHealthIncrease;
        var playerInteractor = FindObjectOfType<PlayerInteractor>();
        if (playerInteractor != null) {
            _playerHealth = playerInteractor.GetComponent<Health>();
        }
    }

    private void FixedUpdate() {
        transform.RotateAround(transform.position, transform.forward, 5f);
    }

    public void Interact() {
        if (PlayerCurrency.Instance.Purchase(cost)) {
            _playerHealth.Heal(1, maxHealthIncrease);
            DestroyImmediate(gameObject);
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public float GetInteractRadius() {
        return 2f;
    }

    public string GetText() {
        return _displayText;
    }
}
