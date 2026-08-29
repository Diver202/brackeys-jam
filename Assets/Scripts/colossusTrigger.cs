using UnityEngine;

public class colossusTrigger : MonoBehaviour {
    public sequenceManagerSceneFour sequenceManager;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        if (!hasTriggered && other.CompareTag("Player")) {
            hasTriggered = true;
            if (sequenceManager != null) {
                sequenceManager.triggerColossus();
            }
        }
    }
}