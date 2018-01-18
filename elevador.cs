using UnityEngine;
using System.Collections;

public class elevador : MonoBehaviour {

    bool active;

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.layer == 8 && !active) {
            GetComponent<Animator>().SetTrigger("go");
            GetComponent<AudioSource>().Play();
            active = true;
        }
	}
}
