using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class Health : MonoBehaviour {
    public delegate void OnDeath();
    public event OnDeath onDeathObservers;
    public delegate void onHealthChange();
    public event onHealthChange onHealthChangeObservers;

    public float EditorCurrentHealth;
    public float EditorMaxHealth;
    private float _currentHealth;
    private float _maxHealth;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    private void Awake() {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogWarning(gameObject.name
                             + " has no rigidbody and has a health component, its health will never lower");
        }
    }

    private void OnValidate() {
        _currentHealth = EditorCurrentHealth;
        _maxHealth = EditorMaxHealth;
    }

    void TakeDamage(float damage) {
        _currentHealth -= damage;
        onHealthChangeObservers?.Invoke();

        if (_currentHealth <= 0) {
            Kill();
        }
    }

    public void Heal(float value, int maxHealthIncrease = -1) {
        _currentHealth += value;
        if (maxHealthIncrease != -1) {
            _maxHealth += maxHealthIncrease;
        }

        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        onHealthChangeObservers?.Invoke();
    }

    public void Kill() {
        onHealthChangeObservers?.Invoke();
        onDeathObservers?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        OnEnter(other.gameObject);
    }

    private void OnCollisionEnter(Collision other) {
        OnEnter(other.gameObject);
    }

    private void OnEnter(GameObject otherCollider) {
        IDamage damage = otherCollider.GetComponent<IDamage>();

        if (damage != null && !damage.GetSafe().Contains(this)) {
            TakeDamage(damage.GetDamage());
        }
    }
}
