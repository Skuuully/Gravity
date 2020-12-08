using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

[Serializable]
public class Node : MonoBehaviour {
    public Vector3 position;
    public int cost = 1;
    public int value = int.MaxValue;
    public Vector3 direction;

    [SerializeField] public int id = 0;
    [NonSerialized] private static int creationId = 0;
    [SerializeField] public List<int> connectedNodes = new List<int>();

    [NonSerialized] private static LayerMask _collisionLayer;

    private void Awake() {
        _collisionLayer = LayerMask.GetMask("PathCollision");
    }

    public void Init(Vector3 position = new Vector3(), params Node[] connected) {
        this.position = position;
        id = creationId;
        creationId++;
        foreach (Node node in connected) {
            ConnectNode(node);
        }

        Collider[] colliders = Physics.OverlapSphere(position, 0.3f, _collisionLayer);
        if (colliders?.Length > 0) {
            cost = 255;
        }
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

    public void SetValue(int value, Node shortNode) {
        this.value = value;
        direction = shortNode.position - position;
        direction.Normalize();
    }

    public bool Active() {
        return cost != 255;
    }
}
