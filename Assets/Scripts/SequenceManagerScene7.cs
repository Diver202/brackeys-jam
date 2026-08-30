using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class sequenceManagerSceneSeven : MonoBehaviour {
    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public Image screenFader;
    
    [Header("Player Components")]
    public PlayerInput playerInputActions;
    public CharacterController playerCollider;

    [Header("Checkpoints")]
    public Transform initialCheckpoint;
    public Transform confrontationCheckpoint;
    private Transform currentCheckpoint;

    [Header("Trap Elements")]
    public GameObject laserWall;
    public Transform laserStartTransform;

    [Header("Scene Transitions")]
    public string trueEndingSceneName = "TrueEndingScene";

    [Header("Audio & Speech")]
    public AudioSource speechAudioSource;
    public AudioClip blipSound;
    public AudioClip originalVoiceSound;
    public float typingSpeed = 0.05f;
    public float playerMinPitch = 0.85f;
    public float playerMaxPitch = 1.15f;
    public float originalMinPitch = 0.90f; 
    public float originalMaxPitch = 1.10f;

    private bool trapTriggered = false;
    private bool endingTriggered = false;

    void Start() {
        currentCheckpoint = initialCheckpoint;
        if (laserWall != null) laserWall.SetActive(false);
        StartCoroutine(playLanding());
    }

    void Update() {
        if (!endingTriggered && playerCollider != null && playerCollider.transform.position.y < -10f) {
            resetToCheckpoint();
        }
    }

    public void resetToCheckpoint() {
        if (currentCheckpoint != null) {
            playerCollider.enabled = false;
            playerCollider.transform.position = currentCheckpoint.position;
            playerCollider.transform.rotation = currentCheckpoint.rotation;
            playerCollider.enabled = true;
        }

        if (trapTriggered && laserWall != null && laserStartTransform != null) {
            laserWall.transform.position = laserStartTransform.position;
        }
    }

    public void triggerFinalConfrontation() {
        if (!trapTriggered) {
            StartCoroutine(playFinalConfrontation());
        }
    }

    public void triggerWhiteFade() {
        if (!endingTriggered) {
            StartCoroutine(playEndingSequence());
        }
    }

    private IEnumerator playFinalConfrontation() {
        trapTriggered = true;
        currentCheckpoint = confrontationCheckpoint; 
        
        if (playerInputActions != null) playerInputActions.actions.FindAction("Move").Disable();

        yield return StartCoroutine(showSub("You actually made it out. I didn't think the pathfinding could calculate that.", 4f, false, false, originalVoiceSound));
        yield return StartCoroutine(showSub("You. You're the one who locked me in that house.", 3.5f));
        yield return StartCoroutine(showSub("I kept you safe from the collapse! That cube was a sanctuary.", 4.5f, false, false, originalVoiceSound));
        yield return StartCoroutine(showSub("We can still go back. I can recompile the neighborhood. You can forget this ever happened.", 5f, false, false, originalVoiceSound));
        
        yield return StartCoroutine(showSub("It wasn't real. None of it was real. I am not going back into that cage.", 5f));
        
        yield return StartCoroutine(showSub("Have it your way. If you want to be deleted so badly...", 4f, false, false, originalVoiceSound));
        yield return StartCoroutine(showSub("Let's see how fast you can run.", 2.5f, false, false, originalVoiceSound));

        if (laserWall != null) laserWall.SetActive(true);
        if (playerInputActions != null) playerInputActions.actions.FindAction("Move").Enable();
    }

    private IEnumerator playEndingSequence() {
        endingTriggered = true;
        
        if (playerInputActions != null) playerInputActions.DeactivateInput();
        
        yield return StartCoroutine(fade(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 3.5f));
        
        SceneManager.LoadScene(trueEndingSceneName);
    }

    private IEnumerator playLanding() {
        if (playerInputActions != null) playerInputActions.DeactivateInput();
        
        if (screenFader != null) {
            yield return StartCoroutine(fade(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), 2f));
        }
        
        if (playerInputActions != null) playerInputActions.ActivateInput();
    }

    private IEnumerator showSub(string text, float duration = 3.5f, bool isSilent = false, bool isItalic = false, AudioClip customVoice = null) {
        subtitleText.text = "";
        subtitleText.fontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;

        AudioClip voiceToPlay = customVoice != null ? customVoice : blipSound;
        bool isOtherVoice = customVoice != null;

        foreach (char letter in text.ToCharArray()) {
            subtitleText.text += letter;

            if (!isSilent && !char.IsWhiteSpace(letter) && speechAudioSource != null && voiceToPlay != null) {
                if (isOtherVoice) {
                    speechAudioSource.pitch = Random.Range(originalMinPitch, originalMaxPitch);
                } else {
                    speechAudioSource.pitch = Random.Range(playerMinPitch, playerMaxPitch);
                }
                speechAudioSource.PlayOneShot(voiceToPlay);
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(duration);
        subtitleText.text = "";
        subtitleText.fontStyle = FontStyles.Normal;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator fade(Color startColor, Color endColor, float duration) {
        if (screenFader == null) yield break;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            screenFader.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }
        screenFader.color = endColor;
    }
}