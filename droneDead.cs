using UnityEngine;
using System.Collections;

public class droneDead : MonoBehaviour {

    public SphereCollider coll;
    public AudioSource sound;

	IEnumerator Start () {
        sound.enabled = true;
        sound.Play();
        int rng = (Random.Range(0,2) > 1) ? 1 : -1 ;
        GetComponent<Rigidbody>().AddTorque(Vector3.right*(rng*10));
        yield return new WaitForSeconds(5f);
        coll.enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
	}
}
