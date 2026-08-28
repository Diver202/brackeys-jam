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

        if (Physics.Raycast(interactionRay, out RaycastHit hitData, interactDistance)) {
            Debug.Log("Ray hit: " + hitData.collider.gameObject.name);
            
            DoorController targetDoor = hitData.collider.GetComponentInParent<DoorController>();
            if (targetDoor != null) {
                targetDoor.toggleDoor();
                return;
            }

            interactableNode targetNode = hitData.collider.GetComponentInParent<interactableNode>();

            if (targetNode != null) {
                Debug.Log("Triggering node: " + targetNode.nodeId);
                targetNode.triggerNode();
            } else {
                Debug.LogWarning("Hit object has no interactableNode script.");
            }
        } else {
            Debug.Log("Ray missed entirely.");
        }
    }
}