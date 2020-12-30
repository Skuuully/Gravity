using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour {
    [SerializeField] private SceneManagement sceneManagement;

    void Start() {
        SceneManager.LoadScene(sceneManagement.Scenes[SceneManagement.MAIN_MENU_INDEX]);
    }
}
