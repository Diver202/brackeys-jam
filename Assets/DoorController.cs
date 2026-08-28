using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {
    public Transform hingePivot;
    public float openAngle = 90f;
    public float swingSpeed = 5f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine activeSwing;

    void Start() {
        closedRotation = hingePivot.localRotation;
        openRotation = Quaternion.Euler(hingePivot.localEulerAngles + Vector3.up * openAngle);
    }

    public void toggleDoor() {
        isOpen = !isOpen;
        if (activeSwing != null) StopCoroutine(activeSwing);
        activeSwing = StartCoroutine(swingDoor(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator swingDoor(Quaternion targetRotation) {
        while (Quaternion.Angle(hingePivot.localRotation, targetRotation) > 0.01f) {
            hingePivot.localRotation = Quaternion.Lerp(hingePivot.localRotation, targetRotation, Time.deltaTime * swingSpeed);
            yield return null;
        }
        hingePivot.localRotation = targetRotation;
    }
}