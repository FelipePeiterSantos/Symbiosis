using UnityEngine;
using System.Collections;

public class cameraOscillation : MonoBehaviour {

	public float amplitudeX = 10.0f;
    public float amplitudeY = 5.0f;
    public float omegaX = 1.0f;
    public float omegaY = 5.0f;
    float index;

    public void Update(){
        index += Time.deltaTime;
        float x = amplitudeX*Mathf.Cos (omegaX*index);
        float y = Mathf.Abs (amplitudeY*Mathf.Sin (omegaY*index));
        transform.localPosition= new Vector3(x,y,0);
    }
}
