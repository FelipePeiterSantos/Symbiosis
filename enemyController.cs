using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class enemyController : MonoBehaviour {

    public Animator anim;
    Transform player;
    public float distance;
    public Transform hudHealth;
    public Transform target;

    public GameObject damageParticle;

    public AnimationCurve damageOrbCurve;
    float maxDistanceOrbDamage = 8f;

    public AudioSource voiceSource;
    public AudioClip[] audioClips;
    int notRepeatHit,notRepeatShout;
    public Transform[] particlePositions;
    public GameObject particleAttack;

	enum actions {idle, walk, atk, atk1, atk2, dashBack, dashForward, dashLeft, dashRight, hit, orbHit, taunt, spawn, noAction};
    actions doAction;
    actions isDoing;

    bool hitDetect;
    RectTransform healthBar;
    NavMeshAgent nav;

	void Start () {
        notRepeatHit = 0;
        notRepeatShout = 0;
        while (player == null){
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        InvokeRepeating("ReactionsUpdate",setupValues.enemy_tickReact,setupValues.enemy_tickReact);
	    doAction = actions.spawn;
        isDoing = actions.spawn;
        hitDetect = false;
        healthBar = hudHealth.FindChild("healthBar").GetComponent<RectTransform>();
        nav = GetComponent<NavMeshAgent>();
    }
	
    void ReactionsUpdate() {
        if (PlayerStatus.distanceToPlayer(transform) <= distance && isDoing == actions.noAction) {
            switch (PlayerStatus.playerIsDoing)
            {
                case "idle":
                    ActionToTake("attack");
                    break;
                case "walk":
                    ActionToTake("randomAtk");
                    break;
                case "attacking":
                    ActionToTake("defend");
                    break;
                case "aim":
                    ActionToTake("attack");
                    break;
                case "dead":
                    if (isDoing != actions.taunt){
                        isDoing = actions.taunt;
                        doAction = actions.taunt;
                    }
                    break;
            }
        }
        else if (PlayerStatus.playerIsDoing == "aim" && PlayerStatus.distanceToPlayer(transform) >= distance+2) {
            ActionToTake("defendDash");
        }
    }

    void ActionToTake(string act) {
        if(PlayerStatus.IsSomeoneAttacking == "yes") {
            act = "defendDash";
        }

        if (act == "attack"){
            if (PlayerStatus.distanceToPlayer(transform) > 3.5f) {
                bool att = (Random.Range(0, 1f) < 0.5f) ? true : false;
                if (att) {
                    isDoing = actions.atk1;
                    doAction = actions.atk1;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    PlayerStatus.IsSomeoneAttacking = "yes";
                }
                else {
                    isDoing = actions.atk2;
                    doAction = actions.atk2;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    PlayerStatus.IsSomeoneAttacking = "yes";
                }
            }
            else if (PlayerStatus.distanceToPlayer(transform) > 2f) {
                bool att = (Random.Range(0, 1f) < 0.5f) ? true : false;
                if (att) {
                    isDoing = actions.atk1;
                    doAction = actions.atk1;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    PlayerStatus.IsSomeoneAttacking = "yes";
                }
                else {
                    isDoing = actions.atk;
                    doAction = actions.atk;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    PlayerStatus.IsSomeoneAttacking = "yes";
                }
            }
            else {
                bool rng = (Random.Range(0, 1f) > 0.5f) ? true : false;
                if (rng) {
                    if (isDoing != actions.dashForward) {
                        isDoing = actions.dashForward;
                        doAction = actions.dashForward;
                    }
                }
                else {
                    if (isDoing != actions.atk) {
                        isDoing = actions.atk;
                        doAction = actions.atk;
                        PlayerStatus.IsSomeoneAttacking = "yes";
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                }
            }
        }
        else if (act == "defend"){
            int rnd = Mathf.FloorToInt(Random.Range(0, 4));
            if (rnd == 0) {
                if (isDoing != actions.dashBack){
                    isDoing = actions.dashBack;
                    doAction = actions.dashBack;
                }
            }
            else if (rnd == 1) {
                if (isDoing != actions.dashLeft){
                    isDoing = actions.dashLeft;
                    doAction = actions.dashLeft;
                }
            }
            else if (rnd == 2){
                if (isDoing != actions.dashRight){
                    isDoing = actions.dashRight;
                    doAction = actions.dashRight;
                }
            }
            else if (rnd == 3){
                if (isDoing != actions.atk){
                    isDoing = actions.atk;
                    doAction = actions.atk;
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
            }
        }
        else if (act == "randomAtk"){
            float rnd = Random.Range(0f, 1f);
            if (rnd > 0.7f) {
                ActionToTake("attack");
            }
            else {
                doAction = actions.idle;
            }
        }
        else if (act == "defendDash"){
            bool rnd = (Random.Range(0f, 1f) < 0.5f) ? true : false;
            if (rnd) {
                if (isDoing != actions.dashLeft){
                    isDoing = actions.dashLeft;
                    doAction = actions.dashLeft;
                }
            }
            else{
                if (isDoing != actions.dashRight){
                    isDoing = actions.dashRight;
                    doAction = actions.dashRight;
                }
            }
        }
    }

	void Update () {
        if(PlayerStatus.distanceToPlayer(transform) > distance && PlayerStatus.distanceToPlayer(transform) < 30 && isDoing != actions.spawn) {
            if (PlayerStatus.playerIsDoing == "aim" && PlayerStatus.distanceToPlayer(transform) >= distance + 2) {
                nav.ResetPath();
            }
            else {
                nav.destination = player.position;
            }
        }
        else if(isDoing == actions.noAction )  {
            nav.ResetPath();
            doAction = actions.idle;
        }

        if(nav.hasPath && isDoing == actions.noAction) {
            doAction = actions.walk;
        }
        if (isDoing == actions.noAction && !nav.hasPath || isDoing == actions.spawn) {
            Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x,transform.position.y,player.position.z) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 40f);
        }

        if(anim) {
            switch (doAction){
                case actions.idle:
                    if(anim.GetInteger("actions") != 0) {
                        anim.SetInteger("actions",0);
                    }
                    break;
                case actions.walk:
                    if(anim.GetInteger("actions") != 1) {
                        anim.SetInteger("actions",1);
                    }
                    break;
                case actions.atk:
                    if(anim.GetInteger("actions") != 3) {
                        anim.SetInteger("actions",3);
                    }
                    break;
                case actions.atk1:
                    if(anim.GetInteger("actions") != 4) {
                        anim.SetInteger("actions", 4);
                    }
                    break;
                case actions.atk2:
                    if(anim.GetInteger("actions") != 5) {
                        anim.SetInteger("actions",5);
                    }
                    break;
                case actions.dashForward:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                    }
                    break;
                case actions.dashBack:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                        transform.rotation *= Quaternion.Euler(0,180,0);
                    }
                    break;
                case actions.dashLeft:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                        transform.rotation *= Quaternion.Euler(0,-90,0);
                    }
                    break;
                case actions.dashRight:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                        transform.rotation *= Quaternion.Euler(0,90,0);
                    }
                    break;
                case actions.hit:
                    if(anim.GetInteger("actions") != 10) {
                        anim.SetInteger("actions",10);
                    }
                    break;
                case actions.taunt:
                    if (anim.GetInteger("actions") != 11){
                        anim.SetInteger("actions", 11);
                    }
                    break;
                case actions.orbHit:
                    if (anim.GetInteger("actions") != 13 && anim.GetInteger("actions") != 14){
                        anim.SetInteger("actions", 13);
                    }
                    break;
                case actions.spawn:
                    if (anim.GetInteger("actions") != 15){
                        anim.SetInteger("actions", 15);
                    }
                    break;
            }
        }
    }

    void LateUpdate() {
        hudHealth.rotation = Camera.main.transform.rotation;
    }

    public void EndAction() {
        doAction = actions.idle;
        isDoing = actions.noAction;
        PlayerStatus.IsSomeoneAttacking = "no";
    }

	void OnTriggerEnter(Collider coll) {
        if(!hitDetect) {
            Vector3 posHit = transform.position + transform.up * 1.5f;
            if (coll.transform.name == "orb") {
                float proximity = damageOrbCurve.Evaluate((coll.transform.position - transform.position).magnitude/maxDistanceOrbDamage);
                if(healthBar != null) {
                    healthBar.sizeDelta -= new Vector2(setupValues.enemy_OrbMaxDamage*proximity,0);
                }
                if (proximity > 0.4f) {
                    Quaternion rot = Quaternion.LookRotation(new Vector3(coll.transform.position.x,transform.position.y,coll.transform.position.z) - transform.position);
                    rot *= Quaternion.Euler(0,180,0);
                    transform.rotation = rot;
                    StartCoroutine(Hit(true));
                    Instantiate(damageParticle,transform.position,Quaternion.identity);
                }
                else {
                    StartCoroutine(Hit(false));
                    Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);
                }
            }
            else if(coll.gameObject.name == "coll") {
                if(healthBar != null) 
                    healthBar.sizeDelta -= new Vector2(setupValues.enemy_playerAttack,0);
                StartCoroutine(Hit(false));
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);
            }
            else if(coll.transform.name.Contains("bulletPf")) { 
                if(healthBar != null) 
                    healthBar.sizeDelta -= new Vector2(setupValues.enemy_playerAttack,0);
                StartCoroutine(Hit(false));
                Instantiate(damageParticle,posHit-(posHit-coll.transform.position)/2f,Quaternion.identity);
            }
        }
    }

    public void LookAtPlayer() {
        Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x,transform.position.y,player.position.z) - transform.position);
        transform.rotation = rot;

    }

    IEnumerator Hit(bool orbHit) {
        hitDetect = true;
        bool isDead = false;
        if(healthBar != null) {
            if(healthBar.sizeDelta.x <= 0) {
                isDead = true;
            }
        }
        if(isDead) {
            GetComponent<enemyDead>().IsDead(orbHit);
            yield return new WaitForSeconds(setupValues.hardHit);
        }
        else if (orbHit) {
            isDoing = actions.orbHit;
            doAction = actions.orbHit;
            Audio_HardHit();
            yield return new WaitForSeconds(setupValues.hardHit);
        }
        else {
            isDoing = actions.hit;
            doAction = actions.hit;
            Audio_Hit();
            yield return new WaitForSeconds(setupValues.hit);
        }

        if(healthBar != null) {
            hitDetect = false;
        }
    }

    public void HitHardStand() {
        if(healthBar != null) {
            if(healthBar.sizeDelta.x > 0) {
                anim.SetInteger("actions", 14);
            }
        }
    }

    public void Audio_Hit() {
        int rng = Random.Range(0,3);
        while(rng == notRepeatHit){
            rng = Random.Range(0,3);
        }
        voiceSource.clip = audioClips[rng];
        voiceSource.Play();
        notRepeatHit = rng;
    }

    public void Audio_HardHit() {
        voiceSource.clip = audioClips[3];
        voiceSource.Play();
    }

    public void Audio_Combo() {
        int rng = Random.Range(0,4);
        while(rng == notRepeatShout){
            rng = Random.Range(0,4);
        }
        voiceSource.clip = audioClips[rng+4];
        voiceSource.Play();
        notRepeatShout = rng; 
    }

    public void Audio_Slash() {
        voiceSource.clip = audioClips[8];
        voiceSource.Play();
    }

    public void Particles(int aux) {
        GameObject aux1 = Instantiate(particleAttack, particlePositions[aux].position, particlePositions[aux].rotation) as GameObject;
        //aux1.transform.parent = particlePositions[aux].transform;
    }
}
