using UnityEngine;
using UnityEngine.InputSystem;

public class playerInteraction : MonoBehaviour {
    public Camera playerCamera;
    public float interactDistance = 10f;

    void Update() {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) {
            fireRaycast();
        }
    }

    private void fireRaycast() {
        Ray interactionRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        
        Debug.DrawRay(interactionRay.origin, interactionRay.direction * interactDistance, Color.red, 3f);

        // Inside your fireRaycast() method:

    if (Physics.Raycast(interactionRay, out RaycastHit hitData, interactDistance)) {
                
        // 1. Check for Narrative Nodes FIRST
        interactableNode targetNode = hitData.collider.GetComponent<interactableNode>();
        if (targetNode != null) {
            targetNode.triggerNode();
            return; // Stops here, physical door ignores the click
        }

        // 2. Check for Normal Doors SECOND
        DoorController targetDoor = hitData.collider.GetComponentInParent<DoorController>();
        if (targetDoor != null) {
            targetDoor.toggleDoor();
            return;
        }
    } else {
            Debug.Log("Ray missed entirely.");
        }
    }
}