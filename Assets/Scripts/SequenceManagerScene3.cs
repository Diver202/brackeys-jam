using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class sequenceManagerSceneThree : MonoBehaviour {
    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public Image screenFader;
    
    [Header("Player Components")]
    public PlayerInput playerInputActions;
    public CharacterController playerCollider;
    public Transform laserCheckpoint;

    [Header("Environment Swap")]
    public GameObject normalPath;
    public GameObject parkourPath;
    public GameObject laserWall;
    public Transform laserStartTransform;
    public Transform redButtonTop;

    [Header("Audio & Speech")]
    public AudioSource speechAudioSource;
    public AudioClip blipSound;
    public AudioClip otherVoiceSound;
    public float typingSpeed = 0.05f;
    public float playerMinPitch = 0.85f;
    public float playerMaxPitch = 1.15f;
    public float otherMinPitch = 0.60f; 
    public float otherMaxPitch = 0.80f;

    private bool laserTriggered = false;

    void Start() {
        if (parkourPath != null) parkourPath.SetActive(false);
        if (laserWall != null) laserWall.SetActive(false);
        StartCoroutine(playLanding());
    }

    void Update() {
        if (playerCollider == null) return;

        float currentY = playerCollider.transform.position.y;

        if (!laserTriggered && currentY < -50f) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else if (laserTriggered && currentY < -280f) {
            resetToCheckpoint();
        }
    }

    public void resetToCheckpoint() {
        if (laserCheckpoint != null) {
            playerCollider.enabled = false;
            playerCollider.transform.position = laserCheckpoint.position;
            playerCollider.transform.rotation = laserCheckpoint.rotation;
            playerCollider.enabled = true;
        }

        if (laserWall != null && laserStartTransform != null) {
            laserWall.transform.position = laserStartTransform.position;
        }
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

    private IEnumerator playLanding() {
        if (playerInputActions != null) playerInputActions.DeactivateInput();
        screenFader.color = new Color(0, 0, 0, 1);
        
        yield return StartCoroutine(fade(1f, 0f, 3f));
        
        if (playerInputActions != null) playerInputActions.ActivateInput();

        yield return StartCoroutine(showSub("Press LShift to sprint", 3.5f, true, true));
    }

    public void triggerDialogue(int dialogueIndex) {
        if (dialogueIndex == 1) StartCoroutine(showSub("What... I can't believe this was my life.", 3f));
        else if (dialogueIndex == 2) StartCoroutine(showSub("How long does this go on for?", 3f));
        else if (dialogueIndex == 3) StartCoroutine(showSub("There are so many of these.", 3f));
    }

    public void processInteraction(string id) {
        if (id == "laserButton" && !laserTriggered) {
            StartCoroutine(playLaserTrap());
        }
    }

    private IEnumerator playLaserTrap() {
        laserTriggered = true;

        if (redButtonTop != null) {
            Vector3 startPos = redButtonTop.localPosition;
            Vector3 endPos = startPos - new Vector3(0, 0.1f, 0); 
            float animTime = 0f;
            float animDuration = 0.2f;

            while (animTime < animDuration) {
                animTime += Time.deltaTime;
                redButtonTop.localPosition = Vector3.Lerp(startPos, endPos, animTime / animDuration);
                yield return null;
            }
            redButtonTop.localPosition = endPos;
        }

        yield return StartCoroutine(showSub("Really? You really thought it would be that simple?", 4f, false, false, otherVoiceSound));
        yield return StartCoroutine(showSub("Who - who are you?", 3f));
        yield return StartCoroutine(showSub("Defecting subject has been located. Neutralise threat.", 4f, false, false, otherVoiceSound));
        
        if (normalPath != null) normalPath.SetActive(false);
        if (parkourPath != null) parkourPath.SetActive(true);
        if (laserWall != null) laserWall.SetActive(true);
    }
}