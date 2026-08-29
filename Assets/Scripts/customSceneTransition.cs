using UnityEngine;
using UnityEngine.SceneManagement;

public class customSceneTransition : MonoBehaviour {
    public string targetSceneName = "Scene5";

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}