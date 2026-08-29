using UnityEngine;

public class walkTrigger : MonoBehaviour {
    public int dialogueIndex;
    public sequenceManagerSceneThree sequenceManager;
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