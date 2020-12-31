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
    [SerializeField] private Canvas gameUiCanvas;
    [SerializeField] private GameObject[] dontDestroy;

    private static SceneManagement _instance;

    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    public static SceneManagement Instance => _instance;
    public List<string> Scenes => _scenes;

    public const int MAIN_MENU_INDEX = 1;
    private const int FIRST_LEVEL_INDEX = 2;

    private void Awake() {
        if (_instance != null) {
            Destroy(this);
            Debug.LogWarning("Attempted to create 2nd instance of scene management singleton");
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(gameUiCanvas);
        foreach (GameObject o in dontDestroy) {
            DontDestroyOnLoad(o);
        }

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for( int i = 0; i < sceneCount; i++ ) {
            _scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
        }
    }

    public void TransitionToNext() {
        if (_currentIndex + 1 >= _scenes.Count) {
            Debug.LogWarning("Attempting to load scene at index: " + (_currentIndex + 1) + " but only: "
                             + _scenes.Count + " available");
            return;
        }

        StartCoroutine(LoadScene());
    }

    public void LoadMainMenu() {
        _currentIndex = MAIN_MENU_INDEX;
        StartCoroutine(LoadScene(_currentIndex));
    }

    public void LoadFirstGameLevel() {
        _currentIndex = FIRST_LEVEL_INDEX;
        StartCoroutine(LoadScene(_currentIndex));
    }

    public void ReloadCurrentLevel() {
        StartCoroutine(LoadScene(_currentIndex));
    }


    public void Quit() {
        Application.Quit();
    }

    IEnumerator LoadScene(int sceneNumber = -1) {
        Time.timeScale = 0f;

        int index; 
        if (sceneNumber > -1) {
            index = sceneNumber;
            _currentIndex = sceneNumber;
        } else {
            _currentIndex++;
            index = _currentIndex;
        }
        var ao = SceneManager.LoadSceneAsync(_scenes[index], LoadSceneMode.Single);
        ao.allowSceneActivation = false;

        fadeAnimator.SetTrigger(FadeOut);
        yield return new WaitForSecondsRealtime(1f);


        while (!ao.isDone) {
            if (ao.progress >= 0.9f) {
                ActivatePlayer(_currentIndex > MAIN_MENU_INDEX);
                ao.allowSceneActivation = true;
            }
            yield return null;
        }

        bool showGameUi = index >= FIRST_LEVEL_INDEX;
        gameUiCanvas.gameObject.SetActive(showGameUi);

        Time.timeScale = 1f;
    }

    private void ActivatePlayer(bool active) {
        var playerControllers = Resources.FindObjectsOfTypeAll<PlayerController>();
        if (playerControllers.Length < 1) {
            Debug.LogWarning("Scene management cannot find the player");
            return;
        }

        var player = playerControllers[0];
        player.gameObject.SetActive(active);
    }
}
