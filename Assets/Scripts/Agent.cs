using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Test;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class Agent : MonoBehaviour, IDamage {
    private static List<Health> agentHealth = new List<Health>();
    [SerializeField] private Walkable pathFindingGrid;
    private Node _flowNode = null;
    private Health _health;

    private Rigidbody _rigidbody;
    public int speed = 600;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        agentHealth.Add(_health);
        var walkables = FindObjectsOfType<Walkable>();
        if (walkables != null) {
            float shortestDist = float.MaxValue;
            pathFindingGrid = walkables[0];
            foreach (var walkable in walkables) {
                float dist = Mathf.Abs((transform.position - walkable.gameObject.transform.position).magnitude);
                if (dist < shortestDist) {
                    pathFindingGrid = walkable;
                    shortestDist = dist;
                }
            }
        }
        
        _health.onDeathObservers += OnDeath;
    }

    private void OnDeath() {
        PlayerCurrency.Instance.Increase(2);
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
        _flowNode = pathFindingGrid.GetNearestNode(transform.position, _flowNode);
        var direction = ShouldUseFlowNodeDirection(_flowNode) ? _flowNode.direction : transform.forward;

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
    bool ShouldUseFlowNodeDirection(Node flowNode) {
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

    public float GetDamage() {
        _health.Kill();
        return 1f;
    }

    public List<Health> GetSafe() {
        return agentHealth;
    }
}
