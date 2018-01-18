using UnityEngine;
using System.Collections;

public class destroySelf : MonoBehaviour {

    public float timedelay;

	IEnumerator Start () {
        yield return new WaitForSeconds(timedelay);
        Destroy(this.gameObject);
	}
}
