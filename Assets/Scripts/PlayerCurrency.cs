using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour {
    public delegate void OnCurrencyChange(int currency);
    public event OnCurrencyChange CurrencyChanged;
    
    private int _currency;

    private static PlayerCurrency _instance;
    public int Currency => _currency;
    public static PlayerCurrency Instance => _instance;

    private void Awake() {
        _instance = this;
        _currency = 0;
    }

    public void Increase(int amount) {
        _currency += amount;
        CurrencyChanged?.Invoke(_currency);
    }

    public bool Purchase(int cost) {
        bool successful = false;

        int afterCurrency = _currency - cost;
        if (afterCurrency >= 0) {
            _currency = afterCurrency;
            successful = true;
        }
        CurrencyChanged?.Invoke(_currency);

        return successful;
    }
}
