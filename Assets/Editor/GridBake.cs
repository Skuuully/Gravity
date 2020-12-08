using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Test;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridBake : EditorWindow {
    public Walkable source;
    
    [MenuItem("Tools/GridBake")]
    static void Init() {
        GridBake gridBake = (GridBake) EditorWindow.GetWindow(typeof(GridBake));
        gridBake.Show();
    }

    private void OnGUI() {
        GUILayout.Label("Drag an object with a walkable component and click bake to bake its nodes");

        source = (Walkable) EditorGUILayout.ObjectField(source, typeof(Walkable), true);

        if (source != null) {
            if (GUILayout.Button("Bake")) {
                Debug.Log("Baking begun");
                source.Bake();
                Debug.Log("Baking finished");
            }

            if (GUILayout.Button("Clear")) {
                source.Clear();
                Debug.Log("Navigation cleared");
            }
        }
    }
}
