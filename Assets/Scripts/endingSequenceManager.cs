using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class simpleEndingManager : MonoBehaviour {
    public Image screenFader; // Assign a pitch-black Image here

    void Start() {
        // Ensure the player can actually click the buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (screenFader != null) {
            StartCoroutine(fadeFromBlack(2f)); // Fades over 2 seconds
        }
    }

    private IEnumerator fadeFromBlack(float duration) {
        Color fadeColor = screenFader.color;
        fadeColor.a = 1f;
        screenFader.color = fadeColor;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            screenFader.color = fadeColor;
            yield return null;
        }
        
        fadeColor.a = 0f;
        screenFader.color = fadeColor;
        
        // Disable the fader object entirely so it doesn't invisibly block your mouse clicks
        screenFader.gameObject.SetActive(false); 
    }

    public void retryLastScene() {
        if (PlayerPrefs.HasKey("savedSceneIndex")) {
            SceneManager.LoadScene(PlayerPrefs.GetInt("savedSceneIndex"));
        } else {
            SceneManager.LoadScene(0); // Fallback to main menu
        }
    }

    public void returnToMainMenu() {
        PlayerPrefs.DeleteKey("savedSceneIndex");
        PlayerPrefs.Save();
        SceneManager.LoadScene(0); 
    }
}