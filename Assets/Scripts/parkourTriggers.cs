using UnityEngine;

public class parkourTriggers : MonoBehaviour {
    public enum triggerType { checkpoint, dialogue, endLevel }
    public triggerType actionOnEnter;
    
    public sequenceManagerSceneFive sequenceManager;
    public Transform spawnLocation; // Only needed if this is a checkpoint

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && sequenceManager != null) {
            
            if (actionOnEnter == triggerType.checkpoint) {
                sequenceManager.updateCheckpoint(spawnLocation != null ? spawnLocation : transform);
            } 
            else if (actionOnEnter == triggerType.dialogue) {
                sequenceManager.triggerDialogue();
            } 
            else if (actionOnEnter == triggerType.endLevel) {
                sequenceManager.finishLevel();
            }
            
            // Turn off the collider so it doesn't fire multiple times
            GetComponent<Collider>().enabled = false;
        }
    }
}