﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test {

    [RequireComponent(typeof(MeshFilter))]
    [Serializable]
    public class Walkable : MonoBehaviour {
        [SerializeField] private List<Node> _nodes;

        [SerializeField] private int subdivide = 0;
        [SerializeField] private float vertexExplode = 1f;
        [SerializeField] private GameObject nodeHolder;

        public Transform playerTransform;

        private void Start() {
            // This should not be required but for some reason unity is resetting the contents of the nodes list
            if (_nodes?.Count < 1) {
                foreach (Transform child in nodeHolder.transform) {
                    _nodes.Add(child.GetComponent<Node>());
                }
            }
        }

        public void Clear() {
            if (nodeHolder == null) {
                Debug.LogError("Attempting to clear but there is no navigation, exiting doing nothing");
                return;
            }

            _nodes = new List<Node>();
            foreach (Transform child in nodeHolder.transform) {
                DestroyImmediate(child.gameObject);
            }
            DestroyImmediate(nodeHolder);
        }
        
        public void Bake() {
            if (nodeHolder != null) {
                Debug.LogError("Attempting to bake the navigation when navigation is baked, will clear existing first");
                Clear();
            }

            nodeHolder = new GameObject("Nodes Holder");
            nodeHolder.transform.parent = transform;

            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            if (subdivide > 0) {
                for (int i = 0; i < subdivide; i++) {
                    MeshHelper.Subdivide4(mesh);
                }
            }
            SetupNavPoints(mesh);
            MergeNodes();
        }

        private void SetupNavPoints(Mesh mesh) {
            for (int i = 0; i < mesh.triangles.Length; i += 3) {
                Vector3 vertex1 = mesh.vertices[mesh.triangles[i]];
                ScaleVertex(ref vertex1, transform, vertexExplode);
                Vector3 vertex2 = mesh.vertices[mesh.triangles[i + 1]];
                ScaleVertex(ref vertex2, transform, vertexExplode);
                Vector3 vertex3 = mesh.vertices[mesh.triangles[i + 2]];
                ScaleVertex(ref vertex3, transform, vertexExplode);
                
                Node node1 = CreateNode(vertex1);
                Node node2 = CreateNode(vertex2);
                Node node3 = CreateNode(vertex3);
                
                float connection12Length = Math.Abs((vertex1 - vertex2).magnitude);
                float connection23Length = Math.Abs((vertex2 - vertex3).magnitude);
                float connection13Length = Math.Abs((vertex1 - vertex3).magnitude);

                if (connection12Length > connection23Length && connection12Length > connection13Length) {
                    node3.ConnectNode(node1);
                    node3.ConnectNode(node2);
                } else if (connection23Length > connection12Length && connection23Length > connection13Length) {
                    node1.ConnectNode(node2);
                    node1.ConnectNode(node3);
                } else if (connection13Length > connection12Length && connection13Length > connection23Length) {
                    node2.ConnectNode(node1);
                    node2.ConnectNode(node3);
                }
            }
        }

        static void ScaleVertex(ref Vector3 vertex, Transform transform, float vertexExplode) {
            Vector3 scale = transform.lossyScale;
            vertex.x *= scale.x + vertexExplode;
            vertex.y *= scale.y + vertexExplode;
            vertex.z *= scale.z + vertexExplode;
            vertex += transform.position;
        }

        private Node CreateNode(Vector3 position) {
            GameObject nodeObject = new GameObject();
            nodeObject.transform.parent = nodeHolder.transform;
            nodeObject.transform.position = position;
            Node node = nodeObject.AddComponent<Node>();
            node.Init();
            nodeObject.name = "Node: " + node.id;

            _nodes.Add(node);
            return node;
        }

        private void SetAllUnreachable() {
            foreach (Node node in _nodes) {
                node.SetValue(int.MaxValue);
            }
        }

        private void MergeNodes() {
            List<Node> removedNodes = new List<Node>();
            foreach (Node node in _nodes) {
                List<Node> matchingPositions = _nodes.FindAll(findNode => (findNode.Position() == node.Position()));
                foreach (Node matchingNode in matchingPositions) {
                    if (node == matchingNode) {
                        continue;
                    }

                    if (removedNodes.Contains(matchingNode) || removedNodes.Contains(node)) {
                        continue;
                    }
                    
                    List<Node> connectedNodes = new List<Node>();
                    foreach (var connectedID in matchingNode.connectedNodes) {
                        connectedNodes.Add(_nodes.Find(node1 => node1.id == connectedID));
                    }
                    node.Merge(matchingNode, connectedNodes);
                    removedNodes.Add(matchingNode);
                }
            }

            foreach (Node node in removedNodes) {
                _nodes.Remove(node);
                DestroyImmediate(node.gameObject);
            }
        }

        private void FixedUpdate() {
            if (playerTransform != null) {
                GenerateFlowField(playerTransform.position);
            }
        }
        
        private void GenerateFlowField(Vector3 targetPosition) {
            Node targetNode = GetNearestNode(targetPosition);
            if (targetNode == null) {
                Debug.LogError("Attempted to generate flow field, however no final node of position found");
                return;
            }

            SetAllUnreachable();
            targetNode.SetValue(0);
            targetNode.SetDirection(targetPosition - targetNode.Position());

            Node currentNode = targetNode;
            List<Node> open = new List<Node> {currentNode};
            while (open.Count > 0) {
                currentNode = open[0];
                open.Remove(currentNode);
                foreach (int connectedNodeID in currentNode.connectedNodes) {
                    Node connectedNode = _nodes.Find(node => node.id == connectedNodeID);
                    if (connectedNode.Active()) {
                        int pathLength = currentNode.Value + connectedNode.Cost;
                        if (pathLength < connectedNode.Value) {
                            if (!open.Contains(connectedNode)) {
                                open.Add(connectedNode);
                            }

                            connectedNode.SetValue(pathLength, currentNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the nearest node to the position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="previousClosest">Optional parameter of the previous closest, limits the search to the node and
        /// its connected node</param>
        /// <returns>The nearest node on the grid</returns>
        public Node GetNearestNode(Vector3 position, Node previousClosest = null) {
            List<Node> nodes;

            if (previousClosest != null) {
                nodes = new List<Node>();
                nodes.Add(_nodes.Find(node => node.id == previousClosest.id));
                foreach (int nodeId in previousClosest.connectedNodes) {
                    nodes.Add(_nodes.Find(node => node.id == nodeId));
                }
            } else {
                nodes = _nodes;
            }

            return GetNearestNode(nodes, position);
        }

        public Node GetNearestNode(List<Node> nodes, Vector3 position, bool allowInactiveNodes = false) {
            Node nearestNode = nodes[0];
            foreach (Node node in nodes) {
                if (allowInactiveNodes || node.Active()) {
                    if ((position - node.Position()).magnitude < (position - nearestNode.Position()).magnitude) {
                        nearestNode = node;
                        if (position == node.Position() || (position - node.Position()).magnitude < 0.3f) {
                            break;
                        }
                    }
                }
            }

            return nearestNode;
        }

        /// <summary>
        /// Responsible for drawing the connections between nodes
        /// </summary>
        private void OnDrawGizmosSelected() {
            if (_nodes?.Count > 0) {
                foreach (Node node in _nodes) {
                    if (!Application.isPlaying) {
                        foreach (int i in node.connectedNodes) {
                            Node connected = _nodes.Find(node1 => node1.id == i);
                            Gizmos.color = node.Active() ? Color.green : Color.black;
                            Gizmos.DrawLine(connected.Position(), node.Position());
                        }
                    } else {
                        if (node.Active()) {
                            Color color = new Color {r = 0f, b = 0f, g = 1f - (node.Value / 7f), a = 1f};
                            if (color.g < 0) {
                                color.g = 0f;
                            }

                            Gizmos.color = color;
                            Gizmos.DrawLine(node.Position(), node.Position() + node.Direction);
                        }
                    }
                }
            }
        }
    }
}