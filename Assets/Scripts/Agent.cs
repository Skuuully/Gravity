using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Test;
using UnityEngine;
using Node = Test.Node;

[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour {
    public Walkable pathFindingGrid;

    private Rigidbody _rigidbody;
    public int speed = 600;

    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Rotate();
        Move();
    }

    void Move() {
        _rigidbody.velocity = transform.forward * speed * Time.deltaTime;
    }
    
    /// <summary>
    /// Rotates the agent in such that its forward will follow the flow nodes
    /// </summary>
    void Rotate() {
        Test.Node flowNode = pathFindingGrid.GetNearestNode(transform.position);
        var direction = ShouldUseFlowNodeDirection(flowNode) ? flowNode.direction : transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);

        int rotateSpeed = 3;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// The flow node direction should not be used if it would cause the agent to walk directly into the body they are
    /// traversing.
    /// </summary>
    /// <param name="flowNode">The flow node to get direction from</param>
    /// <returns>Whether the flow nodes direction should or should not be used</returns>
    bool ShouldUseFlowNodeDirection(Test.Node flowNode) {
        bool shouldUseFlow = true;
        Ray ray = new Ray(transform.position, flowNode.direction);
        if (Physics.Raycast(ray, out var hitInfo, 1.5f)) {
            var walkable = hitInfo.collider.gameObject.GetComponent<Walkable>();
            if (walkable != null) {
                shouldUseFlow = false;
            }
        }

        return shouldUseFlow;
    }
}
