using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class DamagingProjectile : Projectile, IDamage {
    private List<Health> _safe = new List<Health>();

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PathCollision")) {
            Destroy(gameObject);
        }
    }
    
    public float GetDamage() {
        Destroy(gameObject);
        return 1;
    }

    public List<Health> GetSafe() {
        return _safe;
    }
}
