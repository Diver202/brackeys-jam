using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour {
    public Camera playerCamera;
    public float interactDistance = 10f; 

    void Update() {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) {
            checkForDoor();
        }
    }

    private void checkForDoor() {
        Ray interactionRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hitData;

        // Draws a red line in the Scene view for 3 seconds
        Debug.DrawRay(interactionRay.origin, interactionRay.direction * interactDistance, Color.red, 3f);

        if (Physics.Raycast(interactionRay, out hitData, interactDistance)) {
            Debug.Log("Ray hit: " + hitData.collider.gameObject.name);
            
            DoorController targetDoor = hitData.collider.GetComponentInParent<DoorController>();
            
            if (targetDoor != null) {
                targetDoor.toggleDoor();
            } else {
                Debug.LogWarning("No doorController found on the parent hierarchy of the hit object.");
            }
        } else {
            Debug.Log("Ray missed entirely.");
        }
    }
}