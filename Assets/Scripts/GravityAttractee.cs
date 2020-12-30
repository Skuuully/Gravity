using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityAttractee : MonoBehaviour {
    [SerializeField] private float localGravityStrength = 1f;
    private static Vector3 NULL_VECTOR = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private Rigidbody _rigidbody;

    private Vector3 _gravityForce;
    private Vector3 _targetRotation;

    private Vector3 _lastPosition;

    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate() {
        Vector3 position = transform.position;
        if (position != _lastPosition) {
            FindGravityForce();
        }

        _rigidbody.AddForce(_gravityForce);
        _lastPosition = position;
    }

    private void FindGravityForce() {
        GravityInfo gravityInfo = FireRay(transform.up * -1);
        if (gravityInfo.force != NULL_VECTOR) {
            _gravityForce = gravityInfo.force;
            return;
        }

        float closest = float.MaxValue;
        for (float i = -1; i <= 1; i += 0.5f) {
            for (float j = -1; j <= 1; j += 0.5f) {
                Vector3 direction = (transform.up * -1) + (transform.right * i) + (transform.forward * j);
                gravityInfo = FireRay(direction.normalized);

                if (gravityInfo.force != NULL_VECTOR && gravityInfo.distance < closest) {
                    closest = gravityInfo.distance;
                    _gravityForce = gravityInfo.force;
                }
            }
        }
        
        _gravityForce = new Vector3(0f, 0f, 0f);
    }

    private GravityInfo FireRay(Vector3 rayDirection) {
        GravityInfo gravityInfo = new GravityInfo();
        
        Ray ray = new Ray(transform.position, rayDirection);
        //Debug.DrawRay(transform.position, rayDirection * 3, Color.blue);
        if (Physics.Raycast(ray, out var hitInfo)) {
            var attractor = hitInfo.collider.gameObject.GetComponent<GravityAttractor>();
            if (attractor != null) {
                 ChangeRotation(hitInfo.normal);
                 Vector3 gravityForce = hitInfo.normal * (attractor.Gravity * localGravityStrength * -1);
                 gravityInfo.force = gravityForce;
                 gravityInfo.distance = hitInfo.distance;
                 return gravityInfo;
            }
        }

        return gravityInfo;
    }

    private void ChangeRotation(Vector3 normal) {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
    }

    public struct GravityInfo {
        public Vector3 force;
        public float distance;

        GravityInfo(bool initialiseWithDefault = true) {
            distance = float.MaxValue;
            force = NULL_VECTOR;
        }
    }
}
