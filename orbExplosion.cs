using UnityEngine;
using System.Collections;

public class orbExplosion : MonoBehaviour {

    SphereCollider collider;
    int frames;
    float lifeTime;
	void Start () {
        collider = GetComponent<SphereCollider>();
        frames = 0;
        lifeTime += Time.deltaTime;
	}
	
	void Update () {
        lifeTime += Time.deltaTime;
        if(frames < 5) {
            frames++;
        }
        else if(collider.enabled) {
            collider.enabled = false;
        }
        if(lifeTime > 5f) {
            Destroy(this.gameObject);
        }
	}
}
