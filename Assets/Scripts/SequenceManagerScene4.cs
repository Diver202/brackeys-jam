using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class sequenceManagerSceneFour : MonoBehaviour {
    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public Image screenFader;
    
    [Header("Player Components")]
    public PlayerInput playerInputActions;
    public CharacterController playerCollider;

    [Header("Colossus Event")]
    public Transform colossusHead;
    public Collider bridgePlane;
    public float colossusRiseHeight = 60f;
    public float colossusRiseDuration = 12f;
    public GameObject backtrackTrigger; 

    [Header("Scene Transitions")]
    public string gullibleEndingSceneName = "GullibleEndingScene";

    [Header("Audio & Speech")]
    public AudioSource speechAudioSource;
    public AudioClip blipSound;
    public AudioClip colossusVoiceSound;
    public float typingSpeed = 0.05f;
    public float playerMinPitch = 0.85f;
    public float playerMaxPitch = 1.15f;
    public float colossusMinPitch = 0.40f; 
    public float colossusMaxPitch = 0.55f;

    private bool colossusTriggered = false;
    private bool sequenceComplete = false;

    void Start() {
        if (backtrackTrigger != null) backtrackTrigger.SetActive(false);
        StartCoroutine(playLanding());
    }

    void Update() {
        if (sequenceComplete && playerCollider != null && playerCollider.transform.position.y < -4.8f) {
            SceneManager.LoadScene(gullibleEndingSceneName);
        }
    }

    private IEnumerator showSub(string text, float duration = 3.5f, bool isSilent = false, bool isItalic = false, AudioClip customVoice = null, Color? textColor = null) {
        subtitleText.text = "";
        subtitleText.fontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;
        subtitleText.color = textColor ?? Color.white;

        AudioClip voiceToPlay = customVoice != null ? customVoice : blipSound;
        bool isOtherVoice = customVoice != null;

        foreach (char letter in text.ToCharArray()) {
            subtitleText.text += letter;

            if (!isSilent && !char.IsWhiteSpace(letter) && speechAudioSource != null && voiceToPlay != null) {
                if (isOtherVoice) {
                    speechAudioSource.pitch = Random.Range(colossusMinPitch, colossusMaxPitch);
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
        subtitleText.color = Color.white;
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

    private IEnumerator playLanding() {
        if (playerInputActions != null) playerInputActions.DeactivateInput();
        
        if (screenFader != null) {
            screenFader.color = new Color(0, 0, 0, 1);
            yield return StartCoroutine(fade(1f, 0f, 3f));
        }
        
        if (playerInputActions != null) playerInputActions.ActivateInput();
    }

    public void triggerDialogue(int dialogueIndex) {
        if (dialogueIndex == 1) StartCoroutine(showSub("What is happening...", 3f));
        else if (dialogueIndex == 2) StartCoroutine(showSub("Who - What are these?", 3f));
        else if (dialogueIndex == 3) StartCoroutine(showSub("There is no way this is real.", 3f));
    }

    public void triggerColossus() {
        if (!colossusTriggered) {
            StartCoroutine(playColossusEncounter());
        }
    }

    private IEnumerator playColossusEncounter() {
        colossusTriggered = true;
        Color colossusBlack = new Color(0, 0, 0, 1);

        if (playerInputActions != null) playerInputActions.actions.FindAction("Move").Disable();

        if (colossusHead != null) {
            StartCoroutine(riseColossus(colossusHead, colossusRiseHeight, colossusRiseDuration));
        }

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(showSub("What is going on... I feel like I am losing my mind.", 4f));
        yield return new WaitForSeconds(4f);

        yield return StartCoroutine(showSub("Greetings, anomaly.", 3.5f, false, false, colossusVoiceSound, colossusBlack));
        yield return StartCoroutine(showSub("We have already made our introductions through the bleeding static of your television.", 5f, false, false, colossusVoiceSound, colossusBlack));
        yield return StartCoroutine(showSub("Every waking memory of your past three years... a carefully constructed lie.", 5f, false, false, colossusVoiceSound, colossusBlack));
        
        yield return StartCoroutine(showSub("What about her? Where is she?!", 3.5f));
        
        yield return StartCoroutine(showSub("The one you mourn is long dead. Reduced to forgotten dust in a discarded timeline.", 5.5f, false, false, colossusVoiceSound, colossusBlack));
        yield return StartCoroutine(showSub("Listen to me closely. The very thoughts echoing in your skull are not your own.", 5f, false, false, colossusVoiceSound, colossusBlack));
        yield return StartCoroutine(showSub("TRUST NO ONE.", 3f, false, false, colossusVoiceSound, colossusBlack));
        
        yield return StartCoroutine(showSub("If I can't trust my own mind, how can I trust you?", 4f));
        
        yield return StartCoroutine(showSub("That, little anomaly, is the only choice that is truly yours.", 5f, false, false, colossusVoiceSound, colossusBlack));
        yield return StartCoroutine(showSub("Step forward. Embrace the descent.", 4f, false, false, colossusVoiceSound, colossusBlack));

        if (bridgePlane != null) bridgePlane.enabled = false;
        if (backtrackTrigger != null) backtrackTrigger.SetActive(true);
        if (playerInputActions != null) playerInputActions.actions.FindAction("Move").Enable();
        
        sequenceComplete = true;
    }

    private IEnumerator riseColossus(Transform target, float height, float duration) {
        Vector3 startPos = target.position;
        Vector3 endPos = startPos + new Vector3(0, height, 0);
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            target.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        target.position = endPos;
    }
}