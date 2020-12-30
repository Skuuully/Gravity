using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOL : MonoBehaviour {
    [SerializeField] private GameObject[] ddol;
    
    private void Awake() {
        foreach (GameObject o in ddol) {
            DontDestroyOnLoad(o);
        }
    }
}
