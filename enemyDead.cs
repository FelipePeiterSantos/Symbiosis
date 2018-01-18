using UnityEngine;
using System.Collections;

public class enemyDead : MonoBehaviour {

    public GameObject hpBar;
    public Rigidbody rig;
    public NavMeshAgent nav;
    public CapsuleCollider cap;
    public Animator anim;
    public enemyController script;
    public GameObject shadowCast;
    public AudioClip dead;

    int despawn = 0;

    public void Dead() {
        IsDead(false);
    }

    public void IsDead(bool withOrb) {
        despawn = 0;
        transform.tag = "Untagged";
        if(withOrb) {
            anim.SetInteger("actions", 13);
        }
        else {
            anim.SetInteger("actions", 12);
        }
        rig.isKinematic = true;
        script.enabled = false;
        AudioSource aSource = GetComponent<AudioSource>();
        aSource.clip = dead;
        aSource.Play();
        StartCoroutine("InsideGround");
    }

    void Update() {
        if(despawn == 1) {
            transform.position += -Vector3.up*Time.deltaTime;
        }
        
    }

    IEnumerator InsideGround() {
        
        yield return new WaitForSeconds(.1f); 
        Destroy(hpBar.gameObject);
        yield return new WaitForSeconds(5f);
        nav.enabled = false;
        Destroy(shadowCast.gameObject);
        cap.enabled = false;
        despawn = 1;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
