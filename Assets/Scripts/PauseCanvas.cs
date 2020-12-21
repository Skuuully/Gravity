using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

public class PauseCanvas : MonoBehaviour {
    private bool _paused;
    public bool Paused => _paused;

    public void ToggleActive() {
        _paused = !_paused;
        gameObject.SetActive(_paused);
        Time.timeScale = _paused ? 0f : 1f;
    }
    
    public void Activate(bool active = true) {
        gameObject.SetActive(active);
        Time.timeScale = active ? 1f : 0f;
        _paused = active;
    }
}
