using UnityEngine;
using System.Collections;

public class bulletPlayerController : MonoBehaviour {

    public GameObject explode;

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.tag != "Player") {
            explode.transform.parent = null;
            explode.SetActive(true);
            Destroy(this.gameObject);
        }       
    }

    public void Speed(float s) {
        GetComponent<Rigidbody>().velocity = transform.forward * s;
    }
}
