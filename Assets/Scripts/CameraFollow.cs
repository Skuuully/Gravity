using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform Target;
    [SerializeField] private float offset = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        Follow();
        Rotate();
    }

    void Follow() {
        Vector3 targetPosition = Target.position + (offset * Target.up);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
    }

    void Rotate() {
        Quaternion targetRotation = Quaternion.LookRotation(Target.up * -1f, Target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
}
