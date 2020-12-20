using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable {
    private bool _active = true;
    private bool _usable = true;
    private Material _material;
    private Color _activeColor = new Color(0, 1f, 0f, 0.5f);
    private Color _inactiveColor = new Color(1f, 0.5f, 0f, 0.3f);
    [SerializeField] private int cost;

    private void Awake() {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = _active ? _activeColor : _inactiveColor;
        bool active = cost > 0;
        SetActive(active);
    }

    private void SetActive(bool value) {
        _active = value;
        _material.color = _active ? _activeColor : _inactiveColor;
    }

    public void Interact() {
        if (_usable && _active) {
            SceneManagement.Instance.TransitionToNext();
            _usable = false;
        } else {
            if (PlayerCurrency.Instance.Purchase(cost)) {
                SetActive(true);
            }
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public float GetInteractRadius() {
        return 2f;
    }

    public void ShowText(bool visible) {
        
    }
}
