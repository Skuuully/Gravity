using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Test;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class Agent : MonoBehaviour, IDamage {
    private static List<Health> agentHealth = new List<Health>();
    private Health _health;

    private Rigidbody _rigidbody;
    [SerializeField] private GameObject healthSpherePrefab;

    private void Awake() {
        _health = GetComponent<Health>();
        
        agentHealth.Add(_health);
        _health.onDeathObservers += OnDeath;
    }

    private void OnDeath() {
        PlayerCurrency.Instance.Increase(2);
        GameObject healthSphere = Instantiate(healthSpherePrefab, transform.position, transform.rotation);
        var colliders = Physics.OverlapSphere(transform.position, healthSphere.transform.lossyScale.z);
        foreach (Collider collider1 in colliders) {
            var agent = collider1.gameObject.GetComponent<Agent>();
            if (agent != null && agent != this) {
                var health = collider1.gameObject.GetComponent<Health>();
                if (health != null) {
                    health.Heal(1, 1);
                }
            }
        }
    }

    public float GetDamage() {
        _health.Kill();
        return _health.CurrentHealth;
    }

    public List<Health> GetSafe() {
        return agentHealth;
    }
}
