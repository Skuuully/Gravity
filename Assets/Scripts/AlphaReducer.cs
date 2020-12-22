using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AlphaReducer : MonoBehaviour {
    private Material _material;
    private float alphaReductionRate = 3;
    [SerializeField] private bool destroyOnInvisible = true;
    
    private void Awake() {
        _material = gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update() {
        Color c =_material.color;
        c.a -= alphaReductionRate * Time.deltaTime;
        _material.color = c;
        if (destroyOnInvisible && c.a <= 0f) {
            Destroy(gameObject);
        }
    }
}
