using UnityEngine;

public class finalTriggers : MonoBehaviour {
    public enum triggerEvent { confrontation, whiteFade }
    public triggerEvent actionOnEnter;
    
    public sequenceManagerSceneSeven sequenceManager;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && sequenceManager != null) {
            
            if (actionOnEnter == triggerEvent.confrontation) {
                sequenceManager.triggerFinalConfrontation();
            } 
            else if (actionOnEnter == triggerEvent.whiteFade) {
                sequenceManager.triggerWhiteFade();
            }
            
            GetComponent<Collider>().enabled = false;
        }
    }
}