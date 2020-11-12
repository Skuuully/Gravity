using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    private Rigidbody _rigidbody;
    [SerializeField] private float _forwardsForce = 100f;
    
    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        _rigidbody.velocity = transform.forward * (_forwardsForce * Time.deltaTime);
    }
}
