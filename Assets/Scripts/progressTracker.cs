using UnityEngine;
using UnityEngine.SceneManagement;

public class progressTracker : MonoBehaviour {
    void Start() {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("savedSceneIndex", currentScene);
        PlayerPrefs.Save();
    }
}