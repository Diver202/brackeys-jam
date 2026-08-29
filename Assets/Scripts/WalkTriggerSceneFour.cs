using UnityEngine;

public class walkTriggerSceneFour : MonoBehaviour {
    public int dialogueIndex;
    public sequenceManagerSceneFour sequenceManager;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        if (!hasTriggered && other.CompareTag("Player")) {
            hasTriggered = true;
            if (sequenceManager != null) {
                sequenceManager.triggerDialogue(dialogueIndex);
            }
        }
    }
}