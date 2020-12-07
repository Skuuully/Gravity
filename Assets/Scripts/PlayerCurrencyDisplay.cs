using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayerCurrencyDisplay : MonoBehaviour {
    private TMP_Text _text;
    
    private void Awake() {
        _text = GetComponent<TMP_Text>();
    }

    void Start() {
        PlayerCurrency.Instance.CurrencyChanged += InstanceOnCurrencyChanged;
        _text.text = "Money: " + PlayerCurrency.Instance.Currency;
    }

    private void InstanceOnCurrencyChanged(int currency) {
        _text.text = "Money: " + currency;
    }
}
