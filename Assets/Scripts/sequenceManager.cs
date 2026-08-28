using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class sequenceManager : MonoBehaviour {
    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public Image screenFader;
    
    [Header("Player Components")]
    public CharacterController playerCollider;
    public UnityEngine.InputSystem.PlayerInput playerInputActions;
    public Transform sofaSeatPoint;

    [Header("Prompts & Props")]
    public GameObject washroomPrompt;
    public GameObject kitchenPrompt;
    public GameObject sofaPrompt;
    public GameObject sandwichProp;

    [Header("Audio & Speech Settings")]
    public AudioSource actionAudioSource;
    public AudioSource speechAudioSource;
    public AudioClip blipSound;
    public AudioClip alternateVoiceSound;
    public AudioClip washroomSound;
    public AudioClip kitchenSound;
    public float typingSpeed = 0.05f;
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    [Header("TV Settings")]
    public VideoPlayer tvVideoPlayer;

    private int currentStage = 0;

    void Start() {
        washroomPrompt.SetActive(false);
        kitchenPrompt.SetActive(false);
        sofaPrompt.SetActive(false);
        StartCoroutine(playWakeUp());
    }

    private IEnumerator showSub(string text, float duration = 3.5f, bool isSilent = false, bool isItalic = false, AudioClip customVoice = null) {
        subtitleText.text = "";
        subtitleText.fontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;

        AudioClip voiceToPlay = customVoice != null ? customVoice : blipSound;

        foreach (char letter in text.ToCharArray()) {
            subtitleText.text += letter;

            if (!isSilent && !char.IsWhiteSpace(letter) && speechAudioSource != null && voiceToPlay != null) {
                speechAudioSource.pitch = Random.Range(minPitch, maxPitch);
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
        Color c = screenFader.color;
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            screenFader.color = c;
            yield return null;
        }
        c.a = endAlpha;
        screenFader.color = c;
    }

    private IEnumerator playWakeUp() {
        if (playerInputActions != null) playerInputActions.DeactivateInput();

        yield return StartCoroutine(fade(1f, 0f, 3f));

        yield return StartCoroutine(showSub("Another wretched day."));
        yield return StartCoroutine(showSub("Ever since she left, I've been living alone for 3 years."));
        yield return StartCoroutine(showSub("I don't do much anymore."));
        yield return StartCoroutine(showSub("I don't even want to get up."));

        if (playerInputActions != null) playerInputActions.ActivateInput();

        yield return StartCoroutine(showSub("WASD to move. Space to jump. Right click to interact.", 4f, true, true));
        yield return StartCoroutine(showSub("I should get a little proper."));

        currentStage = 1;
        washroomPrompt.SetActive(true);
    }

    public void processInteraction(string id) {
        if (id == "washroom" && currentStage == 1) StartCoroutine(playWashroom());
        else if (id == "kitchen" && currentStage == 2) StartCoroutine(playKitchen());
        else if (id == "sofa" && currentStage == 3) StartCoroutine(playSofa());
        else if (id == "frontDoor") StartCoroutine(playFrontDoor());
    }

    private IEnumerator playFrontDoor() {
        yield return StartCoroutine(showSub("I don't want to go outside.", 3.5f, false, false, alternateVoiceSound));
    }

    private IEnumerator playWashroom() {
        currentStage = 0;
        washroomPrompt.SetActive(false);
        yield return StartCoroutine(fade(0f, 1f, 1.5f));

        if (actionAudioSource != null && washroomSound != null) actionAudioSource.PlayOneShot(washroomSound);
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(fade(1f, 0f, 1.5f));
        yield return StartCoroutine(showSub("I need to eat something."));

        currentStage = 2;
        kitchenPrompt.SetActive(true);
    }

    private IEnumerator playKitchen() {
        currentStage = 0;
        kitchenPrompt.SetActive(false);

        yield return StartCoroutine(showSub("For the past 3 years, there's always been food ready for me in this fridge somehow."));
        yield return StartCoroutine(fade(0f, 1f, 1.5f));

        if (actionAudioSource != null && kitchenSound != null) actionAudioSource.PlayOneShot(kitchenSound);
        
        if (sandwichProp != null) sandwichProp.SetActive(false);
        
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(fade(1f, 0f, 1.5f));
        yield return StartCoroutine(showSub("Time to waste the rest of the day on the couch."));

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
        
        yield return new WaitForSeconds(10f);

        yield return StartCoroutine(fade(0f, 1f, 5f));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}