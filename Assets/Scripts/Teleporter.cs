using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable {
    [Serializable]
    public enum TeleporterType {
        Free,
        Kill,
        Cost
    }

    [SerializeField] private TeleporterType _type; 
    private bool _active = true;
    /// <summary>
    /// Used as a one-shot use variable, once used cannot be used again
    /// </summary>
    private bool _usable = true;
    private Material _material;
    private Color _activeColor = new Color(0, 1f, 0f, 0.5f);
    private Color _inactiveColor = new Color(1f, 0.5f, 0f, 0.3f);
    [SerializeField] private int cost;

    private const string activeText = "'E' to teleport";
    private const string inactiveText = "'E' to activate";
    private const string killText = "Kill all enemies to activate the teleporter";

    private void Awake() {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = _active ? _activeColor : _inactiveColor;
        SetActive(cost == 0 || _type == TeleporterType.Free);

        EnemyTracker enemyTracker = FindObjectOfType<EnemyTracker>();
        if (_type == TeleporterType.Kill && enemyTracker == null) {
            Debug.LogWarning(
                "Teleporter is of type kill but no enemy  tracker can be found, will never be activated");
        }
        if (enemyTracker != null) {
            enemyTracker.enemyCountZero += () => { SetActive(true); };
        }
    }

    private void OnValidate() {
        if (cost == 0) {
            _type = TeleporterType.Free;
        }
    }

    private void SetActive(bool value) {
        _active = value;
        _material.color = value ? _activeColor : _inactiveColor;
    }

    public void Interact() {
        if (!_active) {
            switch (_type) {
                case TeleporterType.Cost:
                    if (PlayerCurrency.Instance.Purchase(cost)) {
                        SetActive(true);
                    }
                    break;
                
                case TeleporterType.Free:
                    SetActive(true);
                    break;
                
                case TeleporterType.Kill:
                    break;
            }
        } else if (_usable) {
            SceneManagement.Instance.TransitionToNext();
            _usable = false;
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public float GetInteractRadius() {
        return 2f;
    }

    public string GetText() {
        string text = "";
        if (_active) {
            text = activeText;
        } else {
            switch (_type) {
                case TeleporterType.Kill:
                    text = killText;
                    break;
                case TeleporterType.Cost:
                    text = inactiveText;
                    text += " costs " + cost;
                    break;
            }
        }

        return text;
    }
}
