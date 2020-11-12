using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    [SerializeField] private static int gridX = 100;
    [SerializeField] private static int gridY = 100;
    private Node[,] grid = new Node[gridX, gridY];

    private void Start() {
        GenerateGrid();
        ConnectNodes();
    }

    void GenerateGrid() {
        for (int i = 0; i < gridX; i++) {
            for (int j = 0; j < gridY; j++) {
                Node node = new Node(new Vector2(i, j));
                grid[i, j] = node;
            }
        }
    }

    void ConnectNodes() {
        for (int i = 0; i < gridX; i++) {
            for (int j = 0; j < gridY; j++) {
                Node currentNode = grid[i, j];
                List<Node> surroundingNodes = GetSurroundingNodes(i, j);
                foreach (Node node in surroundingNodes) {
                    Edge edge = new Edge(currentNode, node);
                    currentNode.connections.Add(edge);
                }
            }
        }
    }

    List<Node> GetSurroundingNodes(int gridXPos, int gridYPos) {
        List<Node> surroundingNodes = new List<Node>();
        
        gridXPos -= 1;
        gridYPos -= 1;
        Tuple<int, int> gridPos = GetOnGrid(gridXPos, gridYPos);
        Node topLeftNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(topLeftNode);

        gridXPos += 1;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node topMiddleNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(topMiddleNode);
        
        gridXPos += 1;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node topRightNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(topRightNode);
        
        gridXPos -= 2;
        gridYPos += 1;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node middleLeftNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(middleLeftNode);
        
        gridXPos += 2;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node middleRightNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(middleRightNode);
        
        gridXPos -= 2;
        gridYPos += 1;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node bottomLeftNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(bottomLeftNode);
        
        gridXPos += 1;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node bottomMiddleNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(bottomMiddleNode);
        
        gridXPos += 1;
        gridPos = GetOnGrid(gridXPos, gridYPos);
        Node bottomRightNode = grid[gridPos.Item1, gridPos.Item2];
        surroundingNodes.Add(bottomRightNode);

        return surroundingNodes;
    }

    Tuple<int, int> GetOnGrid(int gridXPos, int gridYPos) {
        int newGridXPos = gridXPos % gridX;
        newGridXPos = newGridXPos < 0 ? newGridXPos + gridX : newGridXPos;
        int newGridYPos = gridYPos % gridY;
        newGridYPos = newGridYPos < 0 ? newGridYPos + gridY : newGridYPos;
        return new Tuple<int, int>(newGridXPos, newGridYPos);
    }

    private void OnDrawGizmosSelected() {
        for (int i = 0; i < gridX; i++) {
            for (int j = 0; j < gridY; j++) {
                Node node = grid[i, j];
                if (node != null) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(new Vector3(node.position.x, node.position.y), 0.2f);

                    foreach (var connection in node.connections) {
                        Gizmos.color = Color.gray;
                        Gizmos.DrawLine(connection.node1.position, connection.node2.position);
                    }
                }
            }
        }
    }
}

public class Node {
    public Vector2 position = new Vector2();
    public List<Edge> connections = new List<Edge>();

    public Node(Vector2 position) {
        this.position = position;
    }
}

public class Edge {
    public Node node1;
    public Node node2;

    public Edge(Node node1, Node node2) {
        this.node1 = node1;
        this.node2 = node2;
    }
}