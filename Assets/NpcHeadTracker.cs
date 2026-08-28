using UnityEngine;

public class NpcHeadTracker : MonoBehaviour
{
    public Transform playerTarget;
    public Transform headPivot;
    public float rotationSpeed = 5f;

    void Update()
    {
        if (playerTarget != null && headPivot != null)
        {
            trackPlayerPosition();
        }
    }

    private void trackPlayerPosition()
    {
        Vector3 directionToPlayer = playerTarget.position - headPivot.position;
        
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer);
            headPivot.rotation = Quaternion.Slerp(headPivot.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}