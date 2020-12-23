using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlowFieldFollower : MonoBehaviour {
    private Walkable _pathFindingGrid;
    private Node _flowNode;
    private bool _canMove;
    private Rigidbody _rigidbody;
    public int speed = 400;
    private bool _enabled = true;
    public bool Enabled {
        get => _enabled;
        set => _enabled = value;
    }

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        var walkables = FindObjectsOfType<Walkable>();
        if (walkables != null) {
            float shortestDist = float.MaxValue;
            _pathFindingGrid = walkables[0];
            foreach (var walkable in walkables) {
                float dist = Mathf.Abs((transform.position - walkable.gameObject.transform.position).magnitude);
                if (dist < shortestDist) {
                    _pathFindingGrid = walkable;
                    shortestDist = dist;
                }
            }
        }
    }
    
    private void FixedUpdate() {
        Move();
        Rotate();
    }

    void Move() {
        _rigidbody.velocity = (_canMove && _enabled) ? transform.forward * (speed * Time.deltaTime) : Vector3.zero;
    }
    
    /// <summary>
    /// Rotates the agent such that its forward will follow the flow nodes
    /// </summary>
    void Rotate() {
        _flowNode = _pathFindingGrid.GetNearestNode(transform.position, _flowNode);
        var direction = ShouldUseFlowNodeDirection(_flowNode) ? _flowNode.Direction : transform.forward;

        _canMove = true;
        if (Physics.Raycast(new Ray(transform.position, direction), out var raycastHit, 0.6f)) {
            var agent = raycastHit.collider.gameObject.GetComponent<Agent>();
            _canMove = agent == null;
        }

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
        Ray ray = new Ray(transform.position, flowNode.Direction);
        if (Physics.Raycast(ray, out var hitInfo, 1.5f)) {
            var walkable = hitInfo.collider.gameObject.GetComponent<Walkable>();
            if (walkable != null) {
                shouldUseFlow = false;
            }
        }

        return shouldUseFlow;
    }
}
