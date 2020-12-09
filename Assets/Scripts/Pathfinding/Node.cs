using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

[Serializable]
public class Node : MonoBehaviour {
    [SerializeField] private int cost;
    [SerializeField] private int value = int.MaxValue;
    [SerializeField] private Vector3 direction;

    public int Cost => cost;
    public int Value => value;
    public Vector3 Direction => direction;

    public int id = 0;
    [NonSerialized] private static int creationId = 0;
    [SerializeField] public List<int> connectedNodes = new List<int>();
    
    /// <summary>
    /// Array to hold contents of sphere overlap, only size of 1 required as only interested as whether there are any
    /// objects or none
    /// </summary>
    private Collider[] _collisionObjects = new Collider[1];

    /// <summary>
    /// Integer value to show that a node is inactive
    /// </summary>
    private const int INACTIVE_COST = 255; 

    /// <summary>
    /// Initialises the node setting up mandatory data
    /// </summary>
    public void Init() {
        id = creationId;
        creationId++;

        CheckCollisions();
    }

    public void FixedUpdate() {
        CheckCollisions();
    }

    private void CheckCollisions() {
        _collisionObjects[0] = null;
        Physics.OverlapSphereNonAlloc(
            transform.position, 0.3f, _collisionObjects, LayerMask.GetMask("PathCollision"));
        cost = (_collisionObjects[0] != null) ? INACTIVE_COST : 1;
    }

    /// <summary>
    /// Connects the node to another, creating a two way link, from this node to that, and the other node to this.
    /// </summary>
    /// <param name="node"></param>
    public void ConnectNode(Node otherNode) {
        if (otherNode.id == id) {
            Debug.LogError("Tried to connect a node to itself");
        }
        connectedNodes.Add(otherNode.id);
        otherNode.connectedNodes.Add(id);
    }

    /// <summary>
    /// Merges the connections a node into this node. Clears the merged node from other connections adding itself
    /// in its place.
    /// </summary>
    /// <param name="node">The node to clear, replacing its connections with this node</param>
    /// <param name="connections">The nodes connected to the node to clear</param>
    public void Merge(Node node, List<Node> connections) {
        foreach (Node connectedNode in connections) {
            connectedNode.connectedNodes.Remove(node.id);
            if (!connectedNodes.Contains(connectedNode.id)) {
                ConnectNode(connectedNode);
            }
        }

        node.connectedNodes = new List<int>();
    }

    /// <summary>
    /// Sets the value of the node, additionally can pass the node that it is connected to in order to determine in
    /// which this node should point
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="routeNode">An optional node which is the node to go from to get to this node that gave the value</param>
    public void SetValue(int value, Node routeNode = null) {
        this.value = value;
        if (routeNode != null) {
            direction = routeNode.Position() - Position();
            direction.Normalize();
        }
    }

    /// <summary>
    /// Sets the direction of the node and normalizes it
    /// </summary>
    /// <param name="dir">The direction to set</param>
    public void SetDirection(Vector3 dir) {
        direction = dir;
        direction.Normalize();
    }

    /// <summary>
    /// Determines if the node is active, a node may be inactive if something is blocking it e.g. a collision
    /// </summary>
    /// <returns>Whether this node is active</returns>
    public bool Active() {
        return cost != INACTIVE_COST;
    }

    /// <summary>
    /// Utility method to make calls to the position more succinct
    /// </summary>
    /// <returns>The nodes position</returns>
    public Vector3 Position() {
        return transform.position;
    }

    private void OnDrawGizmosSelected() {
        Color color = Active() ? Color.green : Color.red;
        Gizmos.color = color;
        Gizmos.DrawSphere(Position(), 0.1f);
    }
}
