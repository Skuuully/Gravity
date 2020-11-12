using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {
    [SerializeField] private Vector3 gridDimensions;
    [SerializeField] private int gridSpacing;
    private List<Node> gridNodes = new List<Node>();

    // Start is called before the first frame update
    void Start() {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void GenerateGrid() {
        gridDimensions /= 2;

        for (int i = Mathf.FloorToInt(-gridDimensions.x); i < gridDimensions.x; i++) {
            for (int j = Mathf.FloorToInt(-gridDimensions.y); j < gridDimensions.y; j++) {
                for (int k = Mathf.FloorToInt(-gridDimensions.z); k < gridDimensions.z; k++) {
                    Vector3 position = new Vector3(i * gridSpacing, j * gridSpacing, k * gridSpacing);
                    Node newNode = new Node(position);
                    gridNodes.Add(newNode);
                }
            }
        }
        
        Cull();
    }

    void Cull() {
        List<Node> toCull = new List<Node>();
        foreach (var node in gridNodes) {
            Collider[] nearbyColliders = Physics.OverlapSphere(node.position, gridSpacing / 2);
            if (nearbyColliders == null || nearbyColliders.Length < 1) {
                toCull.Add(node);
            }
        }

        foreach (var node in toCull) {
            gridNodes.Remove(node);
        }
    }

    private void OnDrawGizmosSelected() {
        foreach (Node node in gridNodes) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(node.position, 0.3f);
        }
    }
}
