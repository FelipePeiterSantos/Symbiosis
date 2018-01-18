using UnityEngine;
using System.Collections;

public class followPlayer : MonoBehaviour {

    public NavMeshAgent nav;
    public Transform player;

    void Update() {
        if(Vector3.Distance(transform.position,player.position) > 6) {
            nav.destination = player.position;
        }
        else {
            nav.ResetPath();
        }
    }

}
