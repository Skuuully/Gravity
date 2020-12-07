using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Test;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridBake : EditorWindow {
    private List<Walkable> Walkables;
    public Walkable[] source;
    
    [MenuItem("Tools/GridBake")]
    static void Init() {
        GridBake gridBake = (GridBake) EditorWindow.GetWindow(typeof(GridBake));
        gridBake.Show();
    }

    private void OnGUI() {
        GUILayout.Label("Drag an object with a walkable component and click bake to bake its nodes");

        Walkables = NodeList.Walkables;
        if (Walkables == null || Walkables.Count == 0) {
            Walkables = new List<Walkable>();
        }
        Walkables.Add(null);

        source = Walkables.ToArray();
        for (int i = 0; i < Walkables.Count; i++) {
            if (NodeList.Walkables != null && NodeList.Walkables.Count < i) {
                source[i] = (Walkable) EditorGUILayout.ObjectField(source[i], typeof(Walkable), true);
            } else {
                source[i] = (Walkable) EditorGUILayout.ObjectField(Walkables[i], typeof(Walkable), true);
            }
        }
        
        NodeList.Walkables = new List<Walkable>(source);
        NodeList.Walkables.Remove(null);

        if (GUILayout.Button("Bake")) {
            Debug.Log("Baking begun");
            NodeList.Bake();
            Debug.Log("Baking finished");
        }

        if (GUILayout.Button("Clear")) {
            NodeList.Clear();
            Debug.Log("Navigation cleared");
        }
    }
}
