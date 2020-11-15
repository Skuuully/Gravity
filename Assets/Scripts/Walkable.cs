using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Test {

    [RequireComponent(typeof(MeshFilter))]
    public class Walkable : MonoBehaviour {
        private Mesh _mesh;
        [SerializeField] private NodeList _nodeList;

        [SerializeField] private int subdivide = 0;
        [SerializeField] private float vertexExplode = 1f;

        public Transform playerTransform;

        private void FixedUpdate() {
            if (playerTransform != null) {
                GenerateFlowField(playerTransform.position);
            }
        }

        public void BakeNavigation() {
            _mesh = GetComponent<MeshFilter>().sharedMesh;
            NodeList.Bake(_mesh, transform, vertexExplode, subdivide);
        }

        public void DestroyNavigation() {
            NodeList.AllNodes = new List<Node>();
        }

        public Node GetNearestNode(Vector3 position) {
            return _nodeList.GetNearestNode(position);
        }

        void GenerateFlowField(Vector3 targetPosition) {
            _nodeList.GenerateFlowField(targetPosition);
        }

        private void OnDrawGizmos() {
            if (_nodeList?.GetNodes() != null) {
                if (!Application.isPlaying) {
                    foreach (Node node in _nodeList.GetNodes()) {
                        foreach (int i in node.connectedNodes) {
                            Node connected = NodeList.GetNode(i);
                            Gizmos.color = Color.black;
                            Gizmos.DrawLine(connected.position, node.position);
                        }
                    }
                }

                foreach (var node in _nodeList.GetNodes()) {
                    if (node.cost != 255) {
                        Color color = new Color();
                        color.r = 0f;
                        color.b = 0f;
                        color.g = 1f - (node.value / 7f);
                        if (color.g < 0) {
                            color.g = 0f;
                        }

                        color.a = 1f;
                        Gizmos.color = color;
                        Gizmos.DrawSphere(node.position, 0.1f);
                        Gizmos.DrawLine(node.position, node.position + node.direction);
                    }
                }
            }
        }
    }

    [Serializable]
    public class NodeList : ISerializationCallbackReceiver {
        public static List<Node> AllNodes;

        [SerializeField] private List<Node> serializedNodes;

        public static void Bake(Mesh mesh, Transform transform, float vertexExplode, int subdivide) {
            AllNodes = new List<Node>();
            if (subdivide > 0) {
                for (int i = 0; i < subdivide; i++) {
                    MeshHelper.Subdivide4(mesh);
                }
            }
            SetupNavPoints(mesh, transform, vertexExplode);
            MergeNodes();
        }

        static void SetupNavPoints(Mesh mesh, Transform transform, float vertexExplode) {
            AllNodes = new List<Node>();
            for (int i = 0; i < mesh.triangles.Length; i += 3) {
                Vector3 vertex1 = mesh.vertices[mesh.triangles[i]];
                ScaleVertex(ref vertex1, transform, vertexExplode);
                Vector3 vertex2 = mesh.vertices[mesh.triangles[i + 1]];
                ScaleVertex(ref vertex2, transform, vertexExplode);
                Vector3 vertex3 = mesh.vertices[mesh.triangles[i + 2]];
                ScaleVertex(ref vertex3, transform, vertexExplode);

                Node node1 = new Node(vertex1);
                Node node2 = new Node(vertex2);
                Node node3 = new Node(vertex3);
                
                if (VectorsShareTwoPoints(vertex1, vertex2)) {
                    node1.ConnectNode(node2.id);
                }

                if (VectorsShareTwoPoints(vertex1, vertex3)) {
                    node1.ConnectNode(node3.id);
                }

                if (VectorsShareTwoPoints(vertex2, vertex3)) {
                    node2.ConnectNode(node3.id);
                }
            }
        }

        static bool VectorsShareTwoPoints(Vector3 v1, Vector3 v2) {
            return (v1.x == v2.x && v1.y == v2.y) || (v1.x == v2.x && v1.z == v2.z) || (v1.y == v2.y && v1.z == v2.z);
        }

        static void ScaleVertex(ref Vector3 vertex, Transform transform, float vertexExplode) {
            Vector3 scale = transform.lossyScale;
            vertex.x *= scale.x + vertexExplode;
            vertex.y *= scale.y + vertexExplode;
            vertex.z *= scale.z + vertexExplode;
            vertex += transform.position;
        }

        public static void SetAllUnreachable() {
            foreach (Node node in AllNodes) {
                node.value = int.MaxValue;
            }
        }

        public static Node GetNode(int id) {
            return AllNodes.Find(node => node.id == id);
        }
        
        static void MergeNodes() {
            List<Node> removedNodes = new List<Node>();
            foreach (Node node in AllNodes) {
                List<Node> matchingPositions = AllNodes.FindAll(findNode => findNode.position == node.position);
                foreach (Node matchingNode in matchingPositions) {
                    if (node == matchingNode) {
                        continue;
                    }

                    if (removedNodes.Contains(matchingNode) || removedNodes.Contains(node)) {
                        continue;
                    }
                    
                    node.Merge(matchingNode);
                    removedNodes.Add(matchingNode);
                }
            }

            foreach (Node node in removedNodes) {
                AllNodes.Remove(node);
            }
        }
        
        public Node GetNearestNode(Vector3 position, bool allowInactiveNodes = false) {
            Node nearestNode = AllNodes[0];
            foreach (Node node in AllNodes) {
                if (allowInactiveNodes || node.Active()) {
                    if ((position - node.position).magnitude < (position - nearestNode.position).magnitude) {
                        nearestNode = node;
                        if (position == node.position || VectorsClose(position, node.position)) {
                            break;
                        }
                    }
                }
            }

            return nearestNode;
        }

        public List<Node> GetNodes() {
            return AllNodes;
        }

        private bool VectorsClose(Vector3 v1, Vector3 v2) {
            return (v1 - v2).magnitude < 0.3f;
        }
        
        public void GenerateFlowField(Vector3 targetPosition) {
            Node targetNode = GetNearestNode(targetPosition);
            if (targetNode == null) {
                Debug.LogError("Attempted to generate flow field, however no final node of position found");
                return;
            }

            SetAllUnreachable();
            targetNode.value = 0;
            targetNode.direction = targetPosition - targetNode.position;
            targetNode.direction.Normalize();

            Node currentNode = targetNode;
            List<Node> open = new List<Node> {currentNode};
            while (open.Count > 0) {
                currentNode = open[0];
                open.Remove(currentNode);
                foreach (int connectedNodeID in currentNode.connectedNodes) {
                    Node connectedNode = GetNode(connectedNodeID);
                    if (connectedNode.cost != 255) {
                        int pathLength = currentNode.value + connectedNode.cost;
                        if (pathLength < connectedNode.value) {
                            if (!open.Contains(connectedNode)) {
                                open.Add(connectedNode);
                            }

                            connectedNode.SetValue(pathLength, currentNode);
                        }
                    }
                }
            }
        }

        public void OnAfterDeserialize() {
            AllNodes = serializedNodes;
        }

        public void OnBeforeSerialize() {
            serializedNodes = AllNodes;
        }
    }

    [Serializable]
    public class Node {
        public Vector3 position = new Vector3();
        //[NonSerialized] public List<Node> connectedNodes = new List<Node>();
        public int cost = 1;
        public int value = int.MaxValue;
        public Vector3 direction;

        [SerializeField] public int id = 0;
        private static int creationId = 0;
        public List<int> connectedNodes = new List<int>();

        [SerializeField] private static LayerMask collisionLayer = LayerMask.GetMask("PathCollision");
      

        public Node(Vector3 position = new Vector3(), params Node[] connected) {
            this.position = position;
            id = creationId;
            creationId++;
            NodeList.AllNodes.Add(this);
            foreach (Node node in connected) {
                ConnectNode(node.id);
            }

            Collider[] colliders = Physics.OverlapSphere(position, 0.3f, collisionLayer);
            if (colliders?.Length > 0) {
                cost = 255;
            }
        }

        /// <summary>
        /// Connects the node to another, creating a two way link, from this node to that, and the other node to this.
        /// </summary>
        /// <param name="node"></param>
        public void ConnectNode(int nodeID) {
            Node otherNode = NodeList.GetNode(nodeID);
            if (otherNode == this) {
                Debug.LogError("Tried to connect a node to itself");
            }
            connectedNodes.Add(nodeID);
            otherNode.connectedNodes.Add(id);
        }

        /// <summary>
        /// Clears the node from all nodes that it is connected to, and clears its own connected list
        /// </summary>
        public void ClearNodes() {
            foreach (int connectedNode in connectedNodes) {
                NodeList.GetNode(connectedNode).connectedNodes.Remove(id);
            }
            connectedNodes = new List<int>();
        }

        /// <summary>
        /// Merges the connections a node into this node. Clears the merged node from other connections adding itself
        /// in its place.
        /// </summary>
        /// <param name="node">The node to clear, replacing its connections with this node</param>
        public void Merge(Node node) {
            foreach (int i in node.connectedNodes) {
                if (!connectedNodes.Contains(i)) {
                    ConnectNode(i);
                }
            }

            node.ClearNodes();
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

    public static class MeshHelper {
    static List<Vector3> vertices;
    static List<Vector3> normals;
    static List<Color> colors;
    static List<Vector2> uv;
    static List<Vector2> uv1;
    static List<Vector2> uv2;
 
    static List<int> indices;
    static Dictionary<uint,int> newVectices;
 
    static void InitArrays(Mesh mesh)
    {
        vertices = new List<Vector3>(mesh.vertices);
        normals = new List<Vector3>(mesh.normals);
        colors = new List<Color>(mesh.colors);
        uv  = new List<Vector2>(mesh.uv);
        uv1 = new List<Vector2>(mesh.uv2);
        uv2 = new List<Vector2>(mesh.uv2);
        indices = new List<int>();
    }
    static void CleanUp()
    {
        vertices = null;
        normals = null;
        colors = null;
        uv  = null;
        uv1 = null;
        uv2 = null;
        indices = null;
    }
 
    #region Subdivide4 (2x2)
    static int GetNewVertex4(int i1, int i2)
    {
        int newIndex = vertices.Count;
        uint t1 = ((uint)i1 << 16) | (uint)i2;
        uint t2 = ((uint)i2 << 16) | (uint)i1;
        if (newVectices.ContainsKey(t2))
            return newVectices[t2];
        if (newVectices.ContainsKey(t1))
            return newVectices[t1];
 
        newVectices.Add(t1,newIndex);
 
        vertices.Add((vertices[i1] + vertices[i2]) * 0.5f);
        if (normals.Count>0)
            normals.Add((normals[i1] + normals[i2]).normalized);
        if (colors.Count>0)
            colors.Add((colors[i1] + colors[i2]) * 0.5f);
        if (uv.Count>0)
            uv.Add((uv[i1] + uv[i2])*0.5f);
        if (uv1.Count>0)
            uv1.Add((uv1[i1] + uv1[i2])*0.5f);
        if (uv2.Count>0)
            uv2.Add((uv2[i1] + uv2[i2])*0.5f);
 
        return newIndex;
    }
 
 
    /// <summary>
    /// Devides each triangles into 4. A quad(2 tris) will be splitted into 2x2 quads( 8 tris )
    /// </summary>
    /// <param name="mesh"></param>
    public static void Subdivide4(Mesh mesh)
    {
        newVectices = new Dictionary<uint,int>();
 
        InitArrays(mesh);
 
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i + 0];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];
 
            int a = GetNewVertex4(i1, i2);
            int b = GetNewVertex4(i2, i3);
            int c = GetNewVertex4(i3, i1);
            indices.Add(i1);   indices.Add(a);   indices.Add(c);
            indices.Add(i2);   indices.Add(b);   indices.Add(a);
            indices.Add(i3);   indices.Add(c);   indices.Add(b);
            indices.Add(a );   indices.Add(b);   indices.Add(c); // center triangle
        }
        mesh.vertices = vertices.ToArray();
        if (normals.Count > 0)
            mesh.normals = normals.ToArray();
        if (colors.Count>0)
            mesh.colors = colors.ToArray();
        if (uv.Count>0)
            mesh.uv = uv.ToArray();
        if (uv1.Count>0)
            mesh.uv2 = uv1.ToArray();
        if (uv2.Count>0)
            mesh.uv2 = uv2.ToArray();
 
        mesh.triangles = indices.ToArray();
 
        CleanUp();
    }
    #endregion Subdivide4 (2x2)
 
    #region Subdivide9 (3x3)
    static int GetNewVertex9(int i1, int i2, int i3)
    {
        int newIndex = vertices.Count;
 
        // center points don't go into the edge list
        if (i3 == i1 || i3 == i2)
        {
            uint t1 = ((uint)i1 << 16) | (uint)i2;
            if (newVectices.ContainsKey(t1))
                return newVectices[t1];
            newVectices.Add(t1,newIndex);
        }
 
        // calculate new vertex
        vertices.Add((vertices[i1] + vertices[i2] + vertices[i3]) / 3.0f);
        if (normals.Count>0)
            normals.Add((normals[i1] + normals[i2] + normals [i3]).normalized);
        if (colors.Count>0)
            colors.Add((colors[i1] + colors[i2] + colors[i3]) / 3.0f);
        if (uv.Count>0)
            uv.Add((uv[i1] + uv[i2] + uv[i3]) / 3.0f);
        if (uv1.Count>0)
            uv1.Add((uv1[i1] + uv1[i2] + uv1[i3]) / 3.0f);
        if (uv2.Count>0)
            uv2.Add((uv2[i1] + uv2[i2] + uv2[i3]) / 3.0f);
        return newIndex;
    }
 
 
    /// <summary>
    /// Devides each triangles into 9. A quad(2 tris) will be splitted into 3x3 quads( 18 tris )
    /// </summary>
    /// <param name="mesh"></param>
    public static void Subdivide9(Mesh mesh)
    {
        newVectices = new Dictionary<uint,int>();
 
        InitArrays(mesh);
 
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i + 0];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];
 
            int a1 = GetNewVertex9(i1, i2, i1);
            int a2 = GetNewVertex9(i2, i1, i2);
            int b1 = GetNewVertex9(i2, i3, i2);
            int b2 = GetNewVertex9(i3, i2, i3);
            int c1 = GetNewVertex9(i3, i1, i3);
            int c2 = GetNewVertex9(i1, i3, i1);
 
            int d  = GetNewVertex9(i1, i2, i3);
 
            indices.Add(i1);   indices.Add(a1);   indices.Add(c2);
            indices.Add(i2);   indices.Add(b1);   indices.Add(a2);
            indices.Add(i3);   indices.Add(c1);   indices.Add(b2);
            indices.Add(d );   indices.Add(a1);   indices.Add(a2);
            indices.Add(d );   indices.Add(b1);   indices.Add(b2);
            indices.Add(d );   indices.Add(c1);   indices.Add(c2);
            indices.Add(d );   indices.Add(c2);   indices.Add(a1);
            indices.Add(d );   indices.Add(a2);   indices.Add(b1);
            indices.Add(d );   indices.Add(b2);   indices.Add(c1);
        }
 
        mesh.vertices = vertices.ToArray();
        if (normals.Count > 0)
            mesh.normals = normals.ToArray();
        if (colors.Count>0)
            mesh.colors = colors.ToArray();
        if (uv.Count>0)
            mesh.uv = uv.ToArray();
        if (uv1.Count>0)
            mesh.uv2 = uv1.ToArray();
        if (uv2.Count>0)
            mesh.uv2 = uv2.ToArray();
 
        mesh.triangles = indices.ToArray();
 
        CleanUp();
    }
    #endregion Subdivide9 (3x3)
}
}