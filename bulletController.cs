using UnityEngine;
using System.Collections;

public class bulletController : MonoBehaviour {

    GameObject player;
    float cd;

    void Start() {
        InvokeRepeating("DestroyFar",2f,2f);
        player = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(player.transform.position);
        GetComponent<Rigidbody>().velocity = transform.forward * 20;
        cd = 0;
    }

    void DestroyFar() {
        if(Vector3.Distance(transform.position,player.transform.position) > 30) {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerStay() {
        cd += Time.deltaTime;
        if(cd >= 0.02f) {
            Destroy(this.gameObject);
        }
    }

}
