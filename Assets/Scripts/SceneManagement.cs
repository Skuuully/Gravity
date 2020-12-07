using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {
    private List<string> _scenes = new List<string>();
    private int _currentIndex = 0;
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private Canvas _canvas;

    private static SceneManagement _instance;

    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    public static SceneManagement Instance => _instance;

    private void Awake() {
        if (_instance != null) {
            Destroy(this);
            Debug.LogWarning("Attempted to create 2nd instance of scene management singleton");
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_canvas.gameObject);
        var scenes = EditorBuildSettings.scenes;
        foreach (var scene in scenes) {
            _scenes.Add(scene.path);
        }
    }

    public void TransitionToNext() {
        if (_scenes.Count <= _currentIndex) {
            Debug.LogWarning("Attempting to load scene at index: " + (_currentIndex + 1) + " but only: " 
                             + _scenes.Count + " available");
            return;
        }
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        fadeAnimator.SetTrigger(FadeOut);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_scenes[_currentIndex + 1]);
        _currentIndex++;
    }
}
