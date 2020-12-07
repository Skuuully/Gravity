using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class Health : MonoBehaviour, IObservable<float> {
    public delegate void OnDeath();
    public event OnDeath onDeathObservers;

    public float EditorCurrentHealth;
    public float EditorMaxHealth;
    private float _currentHealth;
    private float _maxHealth;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    private List<IObserver<float>> observers = new List<IObserver<float>>();

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
        foreach (var observer in observers) {
            observer.OnNext(_currentHealth);
        }
        
        if (_currentHealth <= 0) {
            Kill();
        }
        
    }

    public void Kill() {
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

    public IDisposable Subscribe(IObserver<float> observer) {
        if (!observers.Contains(observer)) {
            observers.Add(observer);
        }

        return new Unsubscriber(observers, observer);
    }
}

internal class Unsubscriber : IDisposable {
    private List<IObserver<float>> _observers;
    private IObserver<float> _observer;

    internal Unsubscriber(List<IObserver<float>> observers, IObserver<float> observer) {
        _observers = observers;
        _observer = observer;
    }

    public void Dispose() {
        if (_observers.Contains(_observer)) {
            _observers.Remove(_observer);
        }
    }
}
