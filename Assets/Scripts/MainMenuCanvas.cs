using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        quitButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() => Application.Quit());
        playButton.onClick.AddListener(() => SceneManagement.Instance.LoadFirstGameLevel());
    }

    private void test() {
        Debug.Log("AAAAAAAAAAAA");
    }
}
