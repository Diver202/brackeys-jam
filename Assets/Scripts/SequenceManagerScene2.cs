using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class sequenceManagerSceneTwo : MonoBehaviour {
    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public Image screenFader;
    
    [Header("Player Components")]
    public CharacterController playerCollider;
    public UnityEngine.InputSystem.PlayerInput playerInputActions;
    public Transform sofaSeatPoint;

    [Header("Prompts & Interactables")]
    public GameObject washroomPrompt;
    public GameObject kitchenPrompt;
    public GameObject sofaPrompt;
    public GameObject bedPrompt;
    public DoorController frontDoorRig;

    [Header("Audio & Speech Settings")]
    public AudioSource actionAudioSource;
    public AudioSource speechAudioSource;
    public AudioClip blipSound;
    public AudioClip otherVoiceSound;
    public AudioClip washroomSound;
    public AudioClip crashSound; 
    public float typingSpeed = 0.05f;
    public float playerMinPitch = 0.85f;
    public float playerMaxPitch = 1.15f;
    public float otherMinPitch = 0.60f; // Lower pitch for the entity
    public float otherMaxPitch = 0.80f;

    [Header("TV Settings")]
    public VideoPlayer tvVideoPlayer;

    private int currentStage = 0;
    private int doorClickCount = 0;

    void Start() {
        washroomPrompt.SetActive(false);
        kitchenPrompt.SetActive(false);
        sofaPrompt.SetActive(false);
        if (bedPrompt != null) bedPrompt.SetActive(false);
        
        StartCoroutine(playWakeUp());
    }

    private IEnumerator showSub(string text, float duration = 3.5f, bool isSilent = false, bool isItalic = false, AudioClip customVoice = null) {
        subtitleText.text = "";
        subtitleText.fontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;

        AudioClip voiceToPlay = customVoice != null ? customVoice : blipSound;
        bool isOtherVoice = customVoice != null;

        foreach (char letter in text.ToCharArray()) {
            subtitleText.text += letter;

            if (!isSilent && !char.IsWhiteSpace(letter) && speechAudioSource != null && voiceToPlay != null) {
                
                // Route the pitch based on the active voice
                if (isOtherVoice) {
                    speechAudioSource.pitch = Random.Range(otherMinPitch, otherMaxPitch);
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

    private IEnumerator fade(float startAlpha, float endAlpha, float duration) {
        Color fadeColor = screenFader.color;
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            screenFader.color = fadeColor;
            yield return null;
        }
        fadeColor.a = endAlpha;
        screenFader.color = fadeColor;
    }

    private IEnumerator playWakeUp() {
        if (playerInputActions != null) playerInputActions.DeactivateInput();

        yield return StartCoroutine(fade(1f, 0f, 3f));

        yield return StartCoroutine(showSub("Another day."));
        yield return StartCoroutine(showSub("Something feels... off."));

        if (playerInputActions != null) playerInputActions.ActivateInput();

        yield return StartCoroutine(showSub("WASD to move. Space to jump. Right click to interact.", 4f, true, true));
        yield return StartCoroutine(showSub("Might as well take a shower."));

        currentStage = 1;
        washroomPrompt.SetActive(true);
    }

    public void processInteraction(string id) {
        if (currentStage == 0) return; 

        if (id == "frontDoor") {
            if (currentStage < 4) {
                StartCoroutine(playEarlyFrontDoor());
                return;
            } else if (currentStage == 4) {
                StartCoroutine(playFrontDoor());
                return;
            }
        }
        
        if (id == "washroom" && currentStage == 1) StartCoroutine(playWashroom());
        else if (id == "kitchen" && currentStage == 2) StartCoroutine(playKitchen());
        else if (id == "sofa" && currentStage == 3) StartCoroutine(playSofa());
        else if (id == "bed" && currentStage == 4) StartCoroutine(playObliviousEnding());
    }

    private IEnumerator playEarlyFrontDoor() {
        int previousStage = currentStage;
        currentStage = 0; 
        yield return StartCoroutine(showSub("I don't want to go outside.", 3f, false, false, otherVoiceSound));
        currentStage = previousStage; 
    }

    private IEnumerator playWashroom() {
        currentStage = 0;
        washroomPrompt.SetActive(false);
        yield return StartCoroutine(fade(0f, 1f, 1.5f));

        if (actionAudioSource != null && washroomSound != null) actionAudioSource.PlayOneShot(washroomSound);
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(fade(1f, 0f, 1.5f));
        yield return StartCoroutine(showSub("The water was peculiarly hot today?"));
        yield return StartCoroutine(showSub("I should get something to eat."));

        currentStage = 2;
        kitchenPrompt.SetActive(true);
    }

    private IEnumerator playKitchen() {
        currentStage = 0;
        kitchenPrompt.SetActive(false);

        yield return StartCoroutine(showSub("Nothing in the fridge..."));
        yield return StartCoroutine(showSub("This has never happened in 3 years."));
        yield return StartCoroutine(showSub("Guess I'll just sit down and turn on the TV."));
        
        currentStage = 3;
        sofaPrompt.SetActive(true);
    }

    private IEnumerator playSofa() {
        currentStage = 0;
        sofaPrompt.SetActive(false);

        if (playerCollider != null) {
            playerCollider.enabled = false;
            
            if (sofaSeatPoint != null) {
                playerCollider.transform.position = sofaSeatPoint.position;
                playerCollider.transform.rotation = sofaSeatPoint.rotation;
            }
        }

        if (playerInputActions != null) {
            playerInputActions.actions.FindAction("Move").Disable();
            playerInputActions.actions.FindAction("Jump").Disable();
        }

        if (tvVideoPlayer != null) {
            tvVideoPlayer.Play();
        } 
        
        yield return new WaitForSeconds(85f);

        if (playerCollider != null) playerCollider.enabled = true;
        if (playerInputActions != null) {
            playerInputActions.actions.FindAction("Move").Enable();
            playerInputActions.actions.FindAction("Jump").Enable();
        }

        yield return StartCoroutine(showSub("This isn't right.", 3f));
        yield return StartCoroutine(showSub("I have to do something about it.", 3f));
        yield return StartCoroutine(showSub("For the first time in 3 years, I want to leave.", 4f, true, true));
        
        currentStage = 4;
        if (bedPrompt != null) bedPrompt.SetActive(true);
    }

    private IEnumerator playFrontDoor() {
        currentStage = 0; 

        if (doorClickCount == 0) {
            yield return StartCoroutine(showSub("I don't want to go outside.", 3f, false, false, otherVoiceSound));
        } 
        else if (doorClickCount == 1) {
            yield return StartCoroutine(showSub("I still do not want to go outside. I want to go to sleep.", 3f, false, false, otherVoiceSound));
        } 
        else if (doorClickCount == 2) {
            yield return StartCoroutine(showSub("Wait. That's not me. Who are you?", 3f));
            yield return StartCoroutine(showSub("I don't want to go outside. I want to go to sleep.", 3f, false, false, otherVoiceSound));
        } 
        else if (doorClickCount == 3) {
            yield return StartCoroutine(showSub("Who is this?", 3f));
            yield return StartCoroutine(showSub("I don't want to go outside. I want to go to sleep.", 3f, false, false, otherVoiceSound));
        } 
        else if (doorClickCount == 4) {
            yield return StartCoroutine(showSub("But I do...", 3f));
            yield return StartCoroutine(showSub("I don't want to go outside.", 3f, false, false, otherVoiceSound));
        } 
        else if (doorClickCount == 5) {
            yield return StartCoroutine(showSub("I WANT TO GO OUTSIDE.", 2f));
            
            if (frontDoorRig != null) frontDoorRig.toggleDoor();
            
            while (playerCollider != null && playerCollider.transform.position.y >= -22f) {
                yield return null; 
            }

            if (screenFader != null) {
                Color hardBlack = screenFader.color;
                hardBlack.a = 1f;
                screenFader.color = hardBlack;
            }

            if (actionAudioSource != null && crashSound != null) {
                actionAudioSource.PlayOneShot(crashSound);
                yield return new WaitForSeconds(crashSound.length + 0.5f); 
            } else {
                yield return new WaitForSeconds(2f);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            yield break; 
        }

        doorClickCount++;
        currentStage = 4; 
    }

    private IEnumerator playObliviousEnding() {
        currentStage = 0;
        if (bedPrompt != null) bedPrompt.SetActive(false);
        
        yield return StartCoroutine(showSub("Maybe I should just go back to sleep...", 4f));
        yield return StartCoroutine(fade(0f, 1f, 4f));
        
        SceneManager.LoadScene("ObliviousEndingScene"); 
    }
}