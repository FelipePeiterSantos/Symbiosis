using UnityEngine;
using System.Collections;

public class monsterController : MonoBehaviour {

    public Animator anim;
    public float distance;
    public Transform hudHealth;
    public Transform damageParticle;
    public AnimationCurve damageOrbCurve;
    public AudioSource voiceSource;
    public AudioClip[] audioClips;
    int noRepeatShout,noRepeatLongShout,noRepeatHit;
    float maxDistanceOrbDamage = 8f;

    enum actions {atk, atk1, death, idle, idle1, idleTaunt, jumpAtk, run, walk, noAction };
    actions doAction;
    actions isDoing;

    Transform player;
    bool hitFeed;
    bool hitDetect;
    int cdHitFeed;
    bool isDead;
    RectTransform healthBar;
    NavMeshAgent nav;

    void Start () {
        isDead = false;
        noRepeatShout = 0;
        noRepeatLongShout = 0;
        noRepeatHit = 0;
        while(player == null){
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        hitDetect = false;
        InvokeRepeating("ReactionsUpdate",setupValues.boss_tickReact,setupValues.boss_tickReact);
	    doAction = actions.idle;
        isDoing = actions.noAction;
        hitFeed = false;
        cdHitFeed = 0;
        healthBar = hudHealth.FindChild("healthBar").GetComponent<RectTransform>();
        nav = GetComponent<NavMeshAgent>();
    }
	
    void ReactionsUpdate() {
        if(PlayerStatus.distanceToPlayer(transform) <= distance && isDoing == actions.noAction) {
            if (PlayerStatus.playerIsDoing != "dead") {
                ActionToTake("attack");
            }
            else {
                if (isDoing != actions.idleTaunt){
                    isDoing = actions.idleTaunt;
                    doAction = actions.idleTaunt;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
            }
        }
    }
    void ActionToTake(string act){
        if (act == "attack"){
            if (PlayerStatus.distanceToPlayer(transform) < 2.25f) {
                if (isDoing != actions.atk1){
                    isDoing = actions.atk1;
                    doAction = actions.atk1;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
            }
            else if (PlayerStatus.distanceToPlayer(transform) < 4f) {
                bool rng = (Random.Range(0f, 1f) < 0.6f) ? false : true;
                if (rng) {
                    if (isDoing != actions.atk){
                        isDoing = actions.atk;
                        doAction = actions.atk;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                }
                else {
                    if (isDoing != actions.atk1){
                        isDoing = actions.atk1;
                        doAction = actions.atk1;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                }
            }
            else if(!isDead) {
                bool jumpAtkRng = (Random.Range(0f,1f) > setupValues.boss_jumpChance) ? false : true;
                if(jumpAtkRng) {
                    if (PlayerStatus.distanceToPlayer(transform) > 4f){
                        nav.destination = player.position;
                    }
                }
                else if (isDoing != actions.jumpAtk){
                    isDoing = actions.jumpAtk;
                    doAction = actions.jumpAtk;
                    Audio_Jump();
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
            }
        }
    }

    void LateUpdate() {
        if(hudHealth != null) {
            hudHealth.rotation = Camera.main.transform.rotation;
        }
        
    }

	void Update () {
        if (Vector3.Distance(transform.position, player.position) > 4f && Vector3.Distance(transform.position, player.position) < 30){
            if(nav.enabled) {
                nav.destination = player.position;
            }
        }
        else if (isDoing == actions.noAction){
            nav.ResetPath();
            doAction = actions.idle;
        }

        if (nav.hasPath && isDoing == actions.noAction){
            doAction = actions.walk;
        }

        if (hitFeed) {
            cdHitFeed++;
            if(cdHitFeed >= 10f) {
                hitFeed = false;
                cdHitFeed = 0;
            }
        }

        if (isDoing == actions.noAction && !nav.hasPath){
            Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }

        if(anim) {
            switch (doAction){
                case actions.atk:
                    if(anim.GetInteger("actions") != 0) {
                        anim.SetInteger("actions",0);
                    }
                    break;
                case actions.atk1:
                    if(anim.GetInteger("actions") != 1) {
                        anim.SetInteger("actions",1);
                    }
                    break;
                case actions.death:
                    if(anim.GetInteger("actions") != 2) {
                        anim.SetInteger("actions",2);
                    }
                    break;
                case actions.idle:
                    if(anim.GetInteger("actions") != 3) {
                        anim.SetInteger("actions", 3);
                    }
                    break;
                case actions.idleTaunt:
                    if(anim.GetInteger("actions") != 4) {
                        anim.SetInteger("actions",4);
                    }
                    break;
                case actions.jumpAtk:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                    }
                    break;
                case actions.run:
                    if(anim.GetInteger("actions") != 7) {
                        anim.SetInteger("actions",7);
                    }
                    break;
                case actions.walk:
                    if(anim.GetInteger("actions") != 8) {
                        anim.SetInteger("actions",8);
                    }
                    break;
            }
        }
	}

    public void EndAction() {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

	void OnTriggerEnter(Collider coll) {
        if(!isDead) {
            Vector3 posHit = transform.position + transform.up * 2f;
            if(coll.transform.name == "orb") {
                float proximity = damageOrbCurve.Evaluate((coll.transform.position - (transform.position+Vector3.up*3f)).magnitude/maxDistanceOrbDamage);
                healthBar.sizeDelta -= new Vector2(setupValues.boss_OrbMaxDamage*proximity,0);
                StartCoroutine("Hit");
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);          
            }
            else if(coll.transform.name.Contains("bulletPf")) {
                healthBar.sizeDelta -= new Vector2(setupValues.boss_droneAttack,0);
                StartCoroutine("Hit");
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);          
            }
            else if (coll.gameObject.name == "coll" && !hitDetect) {
                healthBar.sizeDelta -= new Vector2(setupValues.boss_playerAttack,0);
                StartCoroutine("Hit");
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);
            }
        }
    }

    public void Parred() {
        isDoing = actions.idle;
        doAction = actions.noAction;
    }

    public void JumpAtkSpeed() {
        if(nav.speed != 10f) {
            nav.speed = 10f;
        }
        else if(nav.speed != 0.1f) {
            nav.speed = 0.1f;
        }
    }

    IEnumerator Hit() {
        hitDetect = true;
        Audio_Hit();
        if(healthBar.sizeDelta.x <= 0) {
            StartCoroutine("Dead");
        }
        hitFeed = true;       
        yield return new WaitForSeconds(setupValues.hit);
        hitDetect = false;
    }

    public void Audio_Shout() {
        int rng = Random.Range(0,3);
        while (rng == noRepeatShout) {
            rng = Random.Range(0,3);
        }
        voiceSource.clip = audioClips[rng];
        voiceSource.Play();
        noRepeatShout = rng;
    }

    public void Audio_LongShout() {
        int rng = Random.Range(0,2);
        while (rng == noRepeatLongShout) {
            rng = Random.Range(0,2);
        }
        voiceSource.clip = audioClips[rng+3];
        voiceSource.Play();
        noRepeatLongShout = rng;
    }

    public void Audio_Jump() {
        voiceSource.clip = audioClips[5];
        voiceSource.Play();
    }

    public void Audio_Hit() {
        int rng = Random.Range(0,3);
        while (rng == noRepeatHit) {
            rng = Random.Range(0,3);
        }
        voiceSource.clip = audioClips[rng+6];
        voiceSource.Play();
        noRepeatHit = rng;
    }

    IEnumerator Dead() {
        isDead = true;
        doAction = actions.death;
        isDoing = actions.death;
        yield return new WaitForSeconds(0.1f);
        voiceSource.clip = audioClips[9];
        voiceSource.Play();
        Destroy(hudHealth.gameObject);
        transform.tag = "Untagged";
        nav.enabled = false;
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawn");
        foreach (GameObject item in spawners){
            Destroy(item.gameObject);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject item in enemies){
            if(item != this.gameObject) {
                item.BroadcastMessage("Dead");
            }
        }
        GameObject.FindGameObjectWithTag("WinGame").SendMessage("Win");
    }
}
