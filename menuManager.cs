using UnityEngine;
using System.Collections;

public class menuManager : MonoBehaviour {

    public GameObject[] canvasManager;
    public GameObject commands;

    public GameObject playerObj;
    public Transform cameraPlayer;
    public Camera[] camerasSetup;
    public Animator menuCredits;

    bool startGame;
    int selectCredits = 0;

    void Start() {
        startGame = false;
        selectCredits = 0;
        canvasManager[0].SetActive(true);
        canvasManager[1].SetActive(false);
        cameraPlayer.GetComponent<cameraOrbitController>().enabled = false;
    }

	void Update() {
        if(startGame) {
            transform.position = Vector3.Lerp(transform.position,cameraPlayer.position,Time.deltaTime*2f);
            transform.rotation = Quaternion.Slerp(transform.rotation,cameraPlayer.rotation,Time.deltaTime*5f);
            camerasSetup[0].fieldOfView = Mathf.Lerp(camerasSetup[0].fieldOfView,camerasSetup[1].fieldOfView,Time.deltaTime*2f);
        }
    }

    IEnumerator IsCameraAlign() {
        while(Vector3.Distance(transform.position,cameraPlayer.position) > 0.1f) {
            yield return false;
        }
        while(Mathf.Abs(transform.rotation.y - cameraPlayer.rotation.y) > 0.1f) {
            yield return false;
        }
        while(Mathf.Abs(camerasSetup[0].fieldOfView - camerasSetup[1].fieldOfView) > 0.1f) {
            yield return false;
        }
        yield return true;
        GameStarted();
    }

    void GameStarted() {
        startGame = false;
        Destroy(this.gameObject);
        playerObj.GetComponent<Animator>().SetInteger("actions",0);
        canvasManager[1].SetActive(true);
        cameraPlayer.GetComponent<cameraOrbitController>().enabled = true;
        playerObj.GetComponent<CapsuleCollider>().enabled = true;
        playerObj.GetComponent<characterController>().enabled = true;
        playerObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        if (PlayerPrefs.GetInt("showCmd") == 0) {
            commands.SetActive(true);
            PlayerPrefs.SetInt("showCmd",1);
        }
    }

    public void OnStartButton() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menuCredits.enabled = false;
        playerObj.GetComponent<Animator>().SetInteger("actions",16);
        canvasManager[0].SetActive(false);
        startGame = true;
        StartCoroutine("IsCameraAlign");
    }

    public void OnCreditsButton() {
        menuCredits.SetInteger("menu",1);
    }

    public void OnBackButton() {
        menuCredits.SetInteger("menu",0);
    }

    public void OnExitButton() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
