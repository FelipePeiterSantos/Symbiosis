using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class droneController : MonoBehaviour {

    public Rigidbody rig;
    public Animator anim;
    public GameObject bulletPf;
    public Transform positionInst;
    public Transform hudHealth;
    public NavMeshAgent nav;
    public GameObject damageParticle;
    public AnimationCurve damageOrbCurve;
    float maxDistanceOrbDamage = 8;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public GameObject droneDead;

    enum actions {idle, chargingAtk, dashLeft, dashRight, dashBack, dashForward, noAction};
    actions doAction;
    actions isDoing;

    Transform player;
    RectTransform healthBar;
    bool hitDetect;

	void Start () {
        while (player == null){
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        InvokeRepeating("ReactionsUpdate",setupValues.drone_tickReact,setupValues.drone_tickReact);
        doAction = actions.idle;
        isDoing = actions.noAction;
        healthBar = hudHealth.FindChild("healthBar").GetComponent<RectTransform>();
        hitDetect = false;
	}
	
    void ReactionsUpdate() {
        if (PlayerStatus.distanceToPlayer(transform) <= 20 && isDoing == actions.noAction) {
            if (PlayerStatus.distanceToPlayer(transform) <= 5){
                ActionToTake("defend");
            }
            else {
                switch (PlayerStatus.playerIsDoing)
                {
                    case "idle":
                        ActionToTake("attack");
                        break;
                    case "walk":
                        ActionToTake("attack");
                        break;
                    case "attacking":
                        ActionToTake("defend");
                        break;
                    case "aim":
                        ActionToTake("defend");
                        break;
                }
            }
        }
    }

    public void ActionToTake(string act) {
        float rng = Random.Range(0f,1f);
        if (act == "attack") {
            doAction = actions.chargingAtk;
            Audio_Charging();
        }
        else if (act == "defend") {
            bool leftTrap = Physics.Raycast(transform.position, -transform.right-transform.forward,1f, 1 << LayerMask.NameToLayer("Default"));
            bool rightTrap = Physics.Raycast(transform.position, transform.right-transform.forward,1f, 1 << LayerMask.NameToLayer("Default"));
            if(leftTrap || rightTrap) {
                if(leftTrap && rightTrap) {
                    doAction = actions.dashForward;
                    isDoing = actions.dashForward;
                }
                else if(leftTrap) {
                    if (rng > 0.5f ) {
                        doAction = actions.dashRight;
                        isDoing = actions.dashRight;
                    }
                    else{
                        doAction = actions.dashForward;
                        isDoing = actions.dashForward;
                    }
                }
                else if(rightTrap) {
                    if (rng > 0.5f ) {
                        doAction = actions.dashLeft;
                        isDoing = actions.dashLeft;
                    }
                    else{
                        doAction = actions.dashForward;
                        isDoing = actions.dashForward;
                    }
                }
            }
            else{
                if (rng > 0.6f ) {
                    doAction = actions.dashLeft;
                    isDoing = actions.dashLeft;
                }
                else if (rng > 0.3f){
                    doAction = actions.dashBack;
                    isDoing = actions.dashBack;
                }
                else if(rng > 0f) {
                    doAction = actions.dashRight;
                    isDoing = actions.dashRight;
                }
            }
        }
    }

    void LateUpdate() {
        hudHealth.rotation = Camera.main.transform.rotation;
    }

	void Update () {
        if(PlayerStatus.distanceToPlayer(transform) > 20 && PlayerStatus.distanceToPlayer(transform) <= 40 && isDoing == actions.noAction) {
            nav.enabled = true;
            nav.destination = player.position;
        }
        else if(nav.enabled){
            nav.ResetPath();
            nav.enabled = false;
        }

        if(isDoing == actions.noAction || isDoing == actions.chargingAtk) {
            Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x,transform.position.y,player.position.z) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 10f);
        }
        switch (doAction){
            case actions.idle:
                rig.velocity = Vector3.zero;
                break;;
            case actions.chargingAtk:
                anim.SetBool("charge", true);
                rig.velocity = Vector3.zero;
                isDoing = actions.chargingAtk;
                break;
            case actions.dashLeft:
                rig.velocity = transform.right * -10;
                StartCoroutine("Dash"); 
                break;
            case actions.dashRight:
                rig.velocity = transform.right * 10;
                StartCoroutine("Dash"); 
                break;
            case actions.dashBack:
                rig.velocity = transform.forward * -10;
                StartCoroutine("Dash");
                break;
            case actions.dashForward:
                rig.velocity = transform.forward * 10;
                StartCoroutine("Dash");
                break;
        }
	}

    void OnTriggerEnter(Collider coll) {
        if(!hitDetect) {
            Vector3 posHit = transform.position + transform.up * 1.25f;
            if (coll.transform.name == "orb") {
                float proximity = damageOrbCurve.Evaluate((coll.transform.position - (transform.position+Vector3.up*2f)).magnitude/maxDistanceOrbDamage);
                healthBar.sizeDelta -= new Vector2(setupValues.drone_OrbMaxDamage*proximity,0);
                StartCoroutine("Hit");  
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);           
            }
            else if (coll.gameObject.name == "coll") {
                StartCoroutine("Hit");
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);
                healthBar.sizeDelta -= new Vector2(setupValues.drone_playerAttack,0);
                if(healthBar.sizeDelta.x <= 0) {
                    Dead();
                }
                rig.velocity = transform.forward * -4;
            }
        }
    }

    public void EndAction() {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

    public void Attack() {
        anim.SetBool("charge", false);
        Instantiate(bulletPf,positionInst.position + transform.forward,Quaternion.identity);
        EndAction();
    }

    IEnumerator Dash() {
        yield return new WaitForSeconds(0.95f);
        EndAction();
    }

    IEnumerator Hit() {
        hitDetect = true;
        if(healthBar.sizeDelta.x <= 0) {
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(setupValues.hit);
        hitDetect = false;
    }

    void Audio_Charging() {
        audioSource.clip = audioClips[0];
        audioSource.pitch = 2.5f;
        audioSource.Play();
    }

    public void Dead() {
        droneDead.transform.parent = null;
        droneDead.GetComponent<SphereCollider>().enabled = true;
        droneDead.AddComponent<Rigidbody>();
        droneDead.GetComponent<droneDead>().enabled = true;
        Destroy(gameObject);
    }
}
