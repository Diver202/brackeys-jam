using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakeUpManager : MonoBehaviour {
    public Image screenFader;
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour cameraControlScript;
    public float wakeUpDuration = 3.5f;

    void Start() {
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraControlScript != null) cameraControlScript.enabled = false;
        
        StartCoroutine(playEyeOpeningRoutine());
    }

    private IEnumerator playEyeOpeningRoutine() {
        Color faderColor = screenFader.color;
        faderColor.a = 1f;
        screenFader.color = faderColor;

        float elapsedTime = 0f;

        while (elapsedTime < wakeUpDuration) {
            elapsedTime += Time.deltaTime;
            faderColor.a = Mathf.Lerp(1f, 0f, elapsedTime / wakeUpDuration);
            screenFader.color = faderColor;
            yield return null;
        }

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (cameraControlScript != null) cameraControlScript.enabled = true;
    }
}