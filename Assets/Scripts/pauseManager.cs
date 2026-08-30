using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pauseManager : MonoBehaviour {
    public GameObject pausePanel;
    public PlayerInput playerInput; 
    private bool isPaused = false;

    void Start() {
        if (pausePanel != null) pausePanel.SetActive(false);
        lockCursor();
    }

    void Update() {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (isPaused) resumeGame();
            else pauseGame();
        }
    }

    public void pauseGame() {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        
        if (playerInput != null) playerInput.DeactivateInput();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void resumeGame() {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        
        if (playerInput != null) playerInput.ActivateInput();
        
        lockCursor();
    }

    public void loadMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void lockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}