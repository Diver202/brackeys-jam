using UnityEngine;

public class interactableNode : MonoBehaviour {
    public string nodeId;
    public sequenceManager managerReference;
    public sequenceManagerSceneTwo managerReferenceTwo; // Added slot for Scene 2

    public void triggerNode() {
        if (managerReference != null) {
            managerReference.processInteraction(nodeId);
        }
        if (managerReferenceTwo != null) {
            managerReferenceTwo.processInteraction(nodeId);
        }
    }
}