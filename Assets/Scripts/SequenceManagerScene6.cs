using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class sequenceManagerSceneSix : MonoBehaviour {
    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public Image screenFader;
    
    [Header("Player Components")]
    public PlayerInput playerInputActions;
    public CharacterController playerCollider;
    public Transform currentCheckpoint;

    [Header("Scene Transitions")]
    public string nextSceneName = "Scene7";

    [Header("Audio & Speech")]
    public AudioSource speechAudioSource;
    public AudioClip blipSound;
    public float typingSpeed = 0.05f;
    public float playerMinPitch = 0.85f;
    public float playerMaxPitch = 1.15f;

    private bool isEnding = false;

    void Start() {
        StartCoroutine(playLanding());
    }

    void Update() {
        if (playerCollider == null) return;

        float currentY = playerCollider.transform.position.y;

        if (currentY < -10f) {
            resetToCheckpoint();
        } else if (currentY > 25f && !isEnding) {
            StartCoroutine(playEndSequence());
        }
    }

    private void resetToCheckpoint() {
        if (currentCheckpoint != null) {
            playerCollider.enabled = false;
            playerCollider.transform.position = currentCheckpoint.position;
            playerCollider.transform.rotation = currentCheckpoint.rotation;
            playerCollider.enabled = true;
        }
    }

    private IEnumerator playEndSequence() {
        isEnding = true;
        
        if (playerInputActions != null) playerInputActions.DeactivateInput();
        if (screenFader != null) yield return StartCoroutine(fade(0f, 1f, 2f));
        
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator playLanding() {
        if (playerInputActions != null) playerInputActions.DeactivateInput();
        
        if (screenFader != null) {
            screenFader.color = new Color(0, 0, 0, 1);
            yield return StartCoroutine(fade(1f, 0f, 2f));
        }
        
        if (playerInputActions != null) playerInputActions.ActivateInput();

        yield return StartCoroutine(showSub("I don't understand how things just keep fading to black and I get teleported.", 4f));
        yield return StartCoroutine(showSub("The creator was on a time crunch.", 3f, true, true));
    }

    private IEnumerator showSub(string text, float duration = 3.5f, bool isSilent = false, bool isItalic = false) {
        subtitleText.text = "";
        subtitleText.fontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;

        foreach (char letter in text.ToCharArray()) {
            subtitleText.text += letter;

            if (!isSilent && !char.IsWhiteSpace(letter) && speechAudioSource != null && blipSound != null) {
                speechAudioSource.pitch = Random.Range(playerMinPitch, playerMaxPitch);
                speechAudioSource.PlayOneShot(blipSound);
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(duration);
        subtitleText.text = "";
        subtitleText.fontStyle = FontStyles.Normal;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator fade(float startAlpha, float endAlpha, float duration) {
        Color fadeColor = screenFader.color;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            screenFader.color = fadeColor;
            yield return null;
        }
        fadeColor.a = endAlpha;
        screenFader.color = fadeColor;
    }
}