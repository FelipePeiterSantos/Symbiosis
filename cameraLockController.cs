using UnityEngine;
using System.Collections;

public class cameraLockController : MonoBehaviour {

    public Transform player;
    public Transform enemy;
    public Transform monster;
    public Transform drone;

    public Vector3 posCamera;	

    Transform target;

    void Start() {
        target = enemy;
    }

	void Update () {
        float close = 9999;
        if (close > Vector3.Distance(enemy.position,transform.position)) {
            if(target != enemy) {
                target = enemy;
            }
            close = Vector3.Distance(enemy.position, transform.position);
        }
        if (close > Vector3.Distance(monster.position,transform.position)) {
            if(target != monster) {
                target = monster;
            }
            close = Vector3.Distance(monster.position, transform.position);
        }
        if (close > Vector3.Distance(drone.position,transform.position)) {
            if(target != drone) {
                target = drone;
            }
            close = Vector3.Distance(drone.position, transform.position);
        }
        transform.position = player.position;

        if (Input.GetKeyDown(KeyCode.Escape)) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
	}

    void LateUpdate() {
        transform.LookAt(new Vector3(target.transform.position.x,transform.position.y,target.transform.position.z));
    }
}
