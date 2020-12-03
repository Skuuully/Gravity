using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayerHealthDisplay : MonoBehaviour, IObserver<float> {
    [SerializeField] private Health playerHealth;
    private TMP_Text _text;

    private void Awake() {
        _text = GetComponent<TMP_Text>();
        playerHealth.Subscribe(this);
    }

    private void Start() {
        _text.text = "HP: " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth;
    }

    public void OnNext(float value) {
        _text.text = "HP: " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth;
    }

    public void OnError(Exception error) {
        throw new NotImplementedException();
    }

    public void OnCompleted() {
        throw new NotImplementedException();
    }
}
