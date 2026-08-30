using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuManager : MonoBehaviour {
    public GameObject infoPanel;
    public Button continueButton;
    public int startingSceneIndex = 1; 

    void Start() {
        if (infoPanel != null) {
            infoPanel.SetActive(false);
        }

        if (!PlayerPrefs.HasKey("savedSceneIndex")) {
            continueButton.interactable = false;
        }
    }

    public void startNewGame() {
        PlayerPrefs.SetInt("savedSceneIndex", startingSceneIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(startingSceneIndex);
    }

    public void continueGame() {
        if (PlayerPrefs.HasKey("savedSceneIndex")) {
            int sceneToLoad = PlayerPrefs.GetInt("savedSceneIndex");
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void openInfoMenu() {
        if (infoPanel != null) {
            infoPanel.SetActive(true);
        }
    }

    public void closeInfoMenu() {
        if (infoPanel != null) {
            infoPanel.SetActive(false);
        }
    }
}