using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUi : MonoBehaviour {
    private TMP_Text _interactableText;
    private static GameUi _instance;
    public static GameUi Instance => _instance;
    
    private void Awake() {
        if (_instance != null) {
            Debug.LogWarning("Creating 2nd instance of GameUi singleton");
            Destroy(this);
        }
        _instance = this;
        
        foreach (Transform t in transform) {
            if (t.gameObject.name == "InteractableText") {
                _interactableText = t.gameObject.GetComponent<TMP_Text>();
            }
        }

        if (_interactableText == null) {
            Debug.LogWarning("Unable to set up the interactable text");
        }
    }

    public void ClearInteractableText() {
        _interactableText.text = "";
    }
    
    public void SetInteractableText(String text) {
        _interactableText.text = text;
    }
}
