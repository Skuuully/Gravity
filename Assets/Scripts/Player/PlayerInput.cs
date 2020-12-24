using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    public float X = 0;
    public float Y = 0;
    public bool fire = false;
    public bool rmb = false;
    public bool interact = false;
    public bool cancel = false;

    private PauseCanvas _uiCanvas;
    
    public void Update() {
        cancel = Input.GetButtonUp("Cancel");
        if (cancel) {
            SetupUiCanvas();
            _uiCanvas.ToggleActive();
        }

        if (_uiCanvas != null && _uiCanvas.Paused) {
            return;
        }

        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");
        fire = Input.GetButton("Fire1");
        rmb = Input.GetButtonUp("Fire2");
        interact = Input.GetButtonUp("Interact");
    }

    private void SetupUiCanvas() {
        if (_uiCanvas == null) {
            var uiCanvas = Resources.FindObjectsOfTypeAll<PauseCanvas>();
            if (uiCanvas.Length < 1) {
                Debug.LogWarning("Attempted to get ui canvas but failed");
                return;
            }

            _uiCanvas = uiCanvas[0];
        }
    }
}