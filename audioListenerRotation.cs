using UnityEngine;
using System.Collections;

public class audioListenerRotation : MonoBehaviour {

    public Transform cameraRot;

    void Update () {
        transform.rotation = cameraRot.rotation;
	}
}
