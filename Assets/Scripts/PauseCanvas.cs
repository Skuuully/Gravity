using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using TMPro;
using UnityEngine;

public class PauseCanvas : MonoBehaviour {
    [SerializeField] private TMP_Text displayText;
    private bool _pauseMode = true;
    private Health _playerHealth;

    private bool _paused;
    public bool Paused => _paused;

    private void Awake() {
        gameObject.SetActive(false);
        ToggleState(_pauseMode);
        PlayerController[] player = Resources.FindObjectsOfTypeAll<PlayerController>();
        if (player == null || player.Length < 1) {
            Debug.LogWarning("Player has not been found by the pause canvas");
            return;
        }

        _playerHealth = player[0].GetComponent<Health>();
        if (_playerHealth == null) {
            Debug.LogWarning("Player has been found but has no health component");
        }
        _playerHealth.onDeathObservers += OnPlayerDeath;
    }

    public void ToggleActive() {
        _paused = !_paused;
        gameObject.SetActive(_paused);
        Time.timeScale = _paused ? 0f : 1f;
    }

    public void ToggleState(bool paused) {
        displayText.text = paused ? "PAUSED" : "DEAD";
        _pauseMode = paused;
    }
    
    private void OnPlayerDeath() {
        ToggleActive();
        ToggleState(false);
        _playerHealth.Heal(Mathf.Ceil(_playerHealth.MaxHealth / 2.0f));
    }
}
