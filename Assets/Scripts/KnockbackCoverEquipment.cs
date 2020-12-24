using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Test;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class KnockbackCoverEquipment : MonoBehaviour, IEquipment {
    Timer cooldownTimer = new Timer(5f);

    [SerializeField] private GameObject knockbackPrefab;
    private GameObject knockbackObject = null;
    
    public void Use() {
        if (knockbackObject == null && cooldownTimer.Finished()) {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var raycastHit)) {
                var walkable = raycastHit.collider.gameObject.GetComponent<Walkable>();
                if (walkable != null) {
                    CreateObject(raycastHit.point);
                }
            }
            
        } else {
            RemoveObject();
        }
    }

    void CreateObject(Vector3 position) {
        knockbackObject = Instantiate(knockbackPrefab, position, Quaternion.identity);
        cooldownTimer.Start();
    }
    
    void RemoveObject() {
        Destroy(knockbackObject);
        knockbackObject = null;
    }

    public void Update() {
        if (knockbackObject != null && cooldownTimer.Finished()) {
            RemoveObject();
        }
    }
}
