using UnityEngine;

public class laserChaser : MonoBehaviour {
    public float speed = 5f;
    public Vector3 direction = Vector3.forward;
    public sequenceManagerSceneThree managerReference;
    public sequenceManagerSceneSeven managerReferenceSeven;

    void Update() {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (managerReference != null) managerReference.resetToCheckpoint();
            if (managerReferenceSeven != null) managerReferenceSeven.resetToCheckpoint();
        }
    }
}