using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityAttractee : MonoBehaviour {
    [SerializeField] private float localGravityStrength = 1f;
    private static Vector3 NULL_VECTOR = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private Rigidbody _rigidbody;

    private GravityInfo _gravityInfo;
    private Vector3 _targetRotation;

    private Vector3 _lastPosition;

    [SerializeField] private bool editorAlignToSurface;

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
            UpdateRotation();
        }

        _rigidbody.AddForce(_gravityInfo.force);
        _lastPosition = position;
    }

    private void FindGravityForce() {
        GravityInfo gravityInfo = FireRay(transform.up * -1);
        if (gravityInfo.force != NULL_VECTOR) {
            _gravityInfo = gravityInfo;
            return;
        }

        float closest = float.MaxValue;
        for (float i = -1; i <= 1; i += 0.5f) {
            for (float j = -1; j <= 1; j += 0.5f) {
                Vector3 direction = (transform.up * -1) + (transform.right * i) + (transform.forward * j);
                gravityInfo = FireRay(direction.normalized);

                if (gravityInfo.force != NULL_VECTOR && gravityInfo.distance < closest) {
                    closest = gravityInfo.distance;
                    _gravityInfo = gravityInfo;
                }
            }
        }

        if (Math.Abs(closest - float.MaxValue) < 0.1f) {
            _gravityInfo = new GravityInfo {force = Vector3.zero};
        }
    }

    private GravityInfo FireRay(Vector3 rayDirection) {
        Ray ray = new Ray(transform.position, rayDirection);
        //Debug.DrawRay(transform.position, rayDirection * 3, Color.blue);
        if (Physics.Raycast(ray, out var hitInfo)) {
            var attractor = hitInfo.collider.gameObject.GetComponent<GravityAttractor>();
            if (attractor != null) {
                Vector3 gravityForce = hitInfo.normal * (attractor.Gravity * localGravityStrength * -1);
                return new GravityInfo(gravityForce, hitInfo.normal, hitInfo.distance);
            }
        }

        return new GravityInfo();
    }

    private void UpdateRotation(bool slerp = true) {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _gravityInfo.surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

        if (!slerp) {
            transform.rotation = targetRotation;
        }
    }

    private void OnValidate() {
        if (editorAlignToSurface) {
            AlignToSurface();
            editorAlignToSurface = !editorAlignToSurface;
        }
    }

    private void AlignToSurface() {
        Vector3 surfaceNormal = GetSurfaceNormal();
        if (surfaceNormal != NULL_VECTOR) {
            UpdateRotation(false);
        }
    }

    private Vector3 GetSurfaceNormal() {
        for (float i = -1; i < 1; i += 0.5f) {
            for (float j = -1; j < 1; j += 0.5f) {
                for (float k = -1; k < 1; k += 0.5f) {
                    Vector3 direction = (transform.forward * i) + (transform.up * j) + (transform.right * k);
                    direction.Normalize();
                    GravityInfo gInfo = FireRay(direction);
                    if (gInfo.surfaceNormal != NULL_VECTOR) {
                        _gravityInfo = gInfo;
                        return gInfo.surfaceNormal;
                    }
                }
            }
        }

        return NULL_VECTOR;
    }

    public class GravityInfo {
        public Vector3 force;
        public Vector3 surfaceNormal;
        public float distance;

        public GravityInfo() {
            distance = float.MaxValue;
            force = NULL_VECTOR;
            surfaceNormal = NULL_VECTOR;
        }

        public GravityInfo(Vector3 force, Vector3 surfaceNormal, float distance) {
            this.force = force;
            this.surfaceNormal = surfaceNormal;
            this.distance = distance;
        }
    }
}
