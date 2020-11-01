using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    public float X = 0;
    public float Y = 0;
    
    public void Update() {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");
    }
}