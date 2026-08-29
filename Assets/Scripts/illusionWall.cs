using UnityEngine;
using UnityEngine.InputSystem;

public class illusionWall : MonoBehaviour {
    public GameObject wallToDisable;
    public PlayerInput playerInput;

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && playerInput != null) {
            
            Vector2 moveInput = playerInput.actions.FindAction("Move").ReadValue<Vector2>();
            
            if (moveInput.y < -0.1f) {
                if (wallToDisable != null) {
                    wallToDisable.SetActive(false);
                }
                
                gameObject.SetActive(false); 
            }
        }
    }
}