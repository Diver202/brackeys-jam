using UnityEngine;

public class interactableNode : MonoBehaviour {
    public string nodeId;
    public sequenceManager managerReference;

    public void triggerNode() {
        if (managerReference != null) {
            managerReference.processInteraction(nodeId);
        }
    }
}