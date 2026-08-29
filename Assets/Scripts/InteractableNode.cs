using UnityEngine;

public class interactableNode : MonoBehaviour {
    public string nodeId;
    public sequenceManager managerReference;
    public sequenceManagerSceneTwo managerReferenceTwo;
    public sequenceManagerSceneThree managerReferenceThree;

    public void triggerNode() {
        if (managerReference != null) managerReference.processInteraction(nodeId);
        if (managerReferenceTwo != null) managerReferenceTwo.processInteraction(nodeId);
        if (managerReferenceThree != null) managerReferenceThree.processInteraction(nodeId);
    }
}