using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityAttractee : MonoBehaviour {
    [SerializeField] private float localGravityStrength = 1f;
    private Vector3 NULL_VECTOR = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private Rigidbody _rigidbody;

    private Vector3 _gravityForce;
    private Vector3 _targetRotation;
    
    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    
    void Update() {
        AttemptGravity();
    }

    private void FixedUpdate() {
        _rigidbody.AddForce(_gravityForce);
    }

    private void AttemptGravity() {
        Vector3 gravityForce = FireRay(transform.up * -1);
        if (gravityForce != NULL_VECTOR) {
            _gravityForce = gravityForce;
            return;
        }
        
        for (float i = -1; i <= 1; i += 0.2f) {
            for (float j = -1; j <= 1; j += 0.2f) {
                Vector3 direction = (transform.up * -1) + (transform.right * i) + (transform.forward * j);
                gravityForce = FireRay(direction.normalized);

                if (gravityForce != NULL_VECTOR) {
                    _gravityForce = gravityForce;
                    return;
                }
            }
        }
        
        _gravityForce = new Vector3(0f, 0f, 0f);
    }

    public Vector3 FireRay(Vector3 rayDirection) {
        
        Ray ray = new Ray(transform.position, rayDirection);
        //Debug.DrawRay(transform.position, rayDirection * 3, Color.blue);
        if (Physics.Raycast(ray, out var hitInfo)) {
            var attractor = hitInfo.collider.gameObject.GetComponent<GravityAttractor>();
            if (attractor != null) {
                 ChangeRotation(hitInfo.normal);
                 Vector3 gravityForce = (attractor.Gravity * localGravityStrength) * (hitInfo.normal * -1);
                 return gravityForce;
            }
        }

        return NULL_VECTOR;
    }

    private void ChangeRotation(Vector3 normal) {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
    }
}
