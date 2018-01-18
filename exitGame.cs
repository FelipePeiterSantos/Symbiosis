using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class exitGame : MonoBehaviour {

    public GameObject win;

	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button6)) {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    #if UNITY_STANDALONE
            SceneManager.LoadScene(0);
    #endif
        }
    }

    public void Win() {
        win.SetActive(true);
    }
}
