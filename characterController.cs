using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour {

    public Animator anim;
    public GameObject cameraMain;
    public AudioSource voiceSource;
    public AudioClip[] audioClips;
    public Transform[] particlePositions;
    public GameObject particleAttack;
    int notRepeatShout,notRepeatHit;
    bool canShout;

    public GameObject hitbox;
    public HUDManager hudScript;
    public GameObject bullet;
    public Transform spawnBUllet;
    public UnityEngine.UI.RawImage aimColor;
    public LayerMask layersBullet;

    public GameObject damageParticle;

    public AnimationCurve damageOrbCurve;
    float maxDistanceOrbDamage = 8f;

    int stamParry, stamDashFwd, stamDashSdBk, stamIniCombo, stamContinueCombo, stamLongAtk = 0;

	enum actions {idle, walk, walkBack, strafeRight, strafeLeft, combo, atk, dashBack, dashForward, dashLeft, dashRight, hit, hitHard, dead, gunAim, noAction};
    actions doAction;
    actions isDoing;

    Transform target;
    bool atkForward;
    bool combo;
    float cdCombo;
    bool hitDetect;
    bool hitIsHard;

    void Start() {
        canShout = false;
        hitIsHard = false;
        GetComponent<CapsuleCollider>().enabled = true;
        doAction = actions.idle;
        isDoing = actions.noAction;
        atkForward = false;
        combo = false;
        cdCombo = 0;
        hitDetect = false;
        hitbox.SetActive(true);
        PlayerStatus.playerIsDoing = "idle";

        stamContinueCombo = setupValues.player_continueCombo;
        stamDashFwd = setupValues.player_dash;
        stamDashSdBk = 25;
        stamIniCombo = setupValues.player_initialCombo;
        stamLongAtk = setupValues.player_longAttack;
        stamParry = 25;
        notRepeatShout = 0;
        notRepeatHit = 0;
    }

	void Update () {
        if(Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Mouse0)) {
            combo = true;
            cdCombo = 0;
        }
        if (combo) {
            cdCombo += Time.deltaTime;
            if(cdCombo >= 0.3f) {
                combo = false;
                cdCombo = 0;
            }
        }

        if(target == null && atkForward) {
            atkForward = false;
        }

        if(!atkForward && target) {
            target = null;
        }

        if(Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKeyDown(KeyCode.Mouse2)) {//HERE
            target = null;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float close = 9999;
            for (int i = 0; i < enemies.Length; i++){
                if (close > Vector3.Distance(enemies[i].transform.position,transform.position)) {
                    close = Vector3.Distance(enemies[i].transform.position, transform.position);
                    if (target != enemies[i].transform) {
                        target = enemies[i].transform;
                    }
                }
            }
            if (target) {
                RaycastHit hit;
                if(!Physics.Raycast(transform.position+Vector3.up*2, target.position-transform.position,out hit, 1<<LayerMask.NameToLayer("Default"))) {
                    atkForward = !atkForward;
                    cameraMain.GetComponent<cameraOrbitController>().LockCamera(!atkForward,target);
                }
            }
        }
        else if(atkForward) {
            RaycastHit hit;
            if(Physics.Raycast(transform.position+Vector3.up*2, (target.position+Vector3.up) - (transform.position+Vector3.up*2),out hit)) {
                if(hit.transform.CompareTag("Enemy") && hit.transform != target) {
                    target = hit.transform;
                    cameraMain.GetComponent<cameraOrbitController>().LockCamera(!atkForward,target);
                }
                else if ((!hit.transform.CompareTag("Enemy") && hit.transform != transform && !hit.transform.name.Contains("spawn") && !hit.transform.name.Contains("(Clone)")) || (hit.transform == target && !hit.transform.CompareTag("Enemy"))) {
                    atkForward = false;
                    //target = null;
                    cameraMain.GetComponent<cameraOrbitController>().LockCamera(!atkForward,target);
                }
            }
        }


        if(isDoing != actions.noAction) {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                if((!atkForward && isDoing != actions.gunAim && isDoing != actions.hitHard) || isDoing == actions.dashForward) {
                    Quaternion rot = Quaternion.LookRotation(new Vector3(-cameraMain.transform.forward.x, transform.forward.y, -cameraMain.transform.forward.z));
                    rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 10f);
                }
            }

        }

        if ((Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Space)) && isDoing != actions.dashForward && isDoing != actions.gunAim && isDoing != actions.hitHard) {//HERE
            if(hudScript.UseStamina(stamDashFwd)) {
                doAction = actions.dashForward;
                isDoing = actions.dashForward;
                //voiceSource.clip = audioClips[11];
                //voiceSource.Play();
            }
        }

        if (isDoing == actions.noAction) {
            if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Mouse0)) {
                if (hudScript.UseStamina(stamIniCombo)) {
                    canShout = true;
                    doAction = actions.combo;
                    isDoing = actions.combo;
                    if (atkForward) {
                        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                    }
                }
            }
            else if (Input.GetAxis("Fire1") <= -0.1f) {
                if (hudScript.UseStamina(stamLongAtk)) {
                    canShout = true;
                    doAction = actions.atk;
                    isDoing = actions.atk;
                    if (atkForward) {
                        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.RightControl)){
                doAction = actions.gunAim;
                isDoing = actions.gunAim;
                cameraMain.GetComponent<cameraOrbitController>().AimCamera(true);
                hudScript.AimFeed(true);
            }
            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                if (atkForward) {
                    Quaternion rot = Quaternion.LookRotation(new Vector3(-cameraMain.transform.forward.x, transform.forward.y, -cameraMain.transform.forward.z));//Quaternion.LookRotation(new Vector3(cameraMain.transform.position.x, transform.position.y, cameraMain.transform.position.z) - transform.position);
                    if (Input.GetAxis("Vertical") >= 0.5f) {
                        doAction = actions.walk;
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    else if (Input.GetAxis("Vertical") <= -0.5f) {
                        doAction = actions.walkBack;
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg + 180, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    else if (Input.GetAxis("Horizontal") <= -0.5f) {
                        doAction = actions.strafeRight;
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg + 90, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    else if (Input.GetAxis("Horizontal") >= 0.5f) {
                        doAction = actions.strafeLeft;
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg - 90, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                }
                else {
                    doAction = actions.walk;
                    Quaternion rot = Quaternion.LookRotation(new Vector3(-cameraMain.transform.forward.x, transform.forward.y, -cameraMain.transform.forward.z));//Quaternion.LookRotation(new Vector3(cameraMain.transform.position.x, transform.position.y, cameraMain.transform.position.z) - transform.position);
                    rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                }
            }
            else {
                doAction = actions.idle;
            }
        }

        if (isDoing == actions.gunAim) {
            RaycastHit lookAtAim;
            Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out lookAtAim, 200f,layersBullet);
            if (lookAtAim.collider != null) {
                if(lookAtAim.transform.gameObject.layer != 0 && lookAtAim.transform != transform) {
                    if(aimColor.color != Color.red) {
                        aimColor.color = Color.red;
                    }
                }
                else {
                    if(aimColor.color != Color.white){
                        aimColor.color = Color.white;
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Mouse0)) {
                if(lookAtAim.transform != transform) {
                    if(hudScript.UseUlti()) {
                        voiceSource.clip = audioClips[10];
                        voiceSource.Play();
                        GameObject bulletIns = Instantiate(bullet, spawnBUllet.position, Quaternion.identity) as GameObject;
                        bulletIns.transform.LookAt(lookAtAim.point);
                        bulletIns.GetComponent<bulletPlayerController>().Speed(setupValues.energyOrb_speed);
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.RightControl)){
                isDoing = actions.noAction;
                cameraMain.GetComponent<cameraOrbitController>().AimCamera(false);
                hudScript.AimFeed(false);
                atkForward = false;
                //target = null;
            }
            else {
                transform.rotation = Quaternion.Euler(0, cameraMain.transform.rotation.eulerAngles.y, 0);
            }
        }
        if (anim) {
            switch (doAction){
                case actions.idle:
                    if(anim.GetInteger("actions") != 0) {
                        PlayerStatus.playerIsDoing = "idle";
                        anim.SetInteger("actions",0);
                    }
                    break;
                case actions.walk:
                    if(anim.GetInteger("actions") != 1) {
                        PlayerStatus.playerIsDoing = "walk";
                        anim.SetInteger("actions",1);
                    }
                    break;
                case actions.combo:
                    if(anim.GetInteger("actions") != 3) {
                        PlayerStatus.playerIsDoing = "attacking";
                        anim.SetInteger("actions",3);
                    }
                    break;
                case actions.atk:
                    if(anim.GetInteger("actions") != 4) {
                        PlayerStatus.playerIsDoing = "attacking";
                        anim.SetInteger("actions",4);
                    }
                    break;
                case actions.dashForward:
                    if(anim.GetInteger("actions") != 6) {
                        PlayerStatus.playerIsDoing = "dash";
                        anim.SetInteger("actions",6);
                    }
                    break;
                case actions.dashBack:
                    if(anim.GetInteger("actions") != 7) {
                        PlayerStatus.playerIsDoing = "dash";
                        anim.SetInteger("actions",7);
                    }
                    break;
                case actions.dashLeft:
                    if(anim.GetInteger("actions") != 8) {
                        PlayerStatus.playerIsDoing = "dash";
                        anim.SetInteger("actions",8);
                    }
                    break;
                case actions.dashRight:
                    if(anim.GetInteger("actions") != 9) {
                        PlayerStatus.playerIsDoing = "dash";
                        anim.SetInteger("actions",9);
                    }
                    break;
                case actions.hit:
                    if(anim.GetInteger("actions") != 10) {
                        anim.SetInteger("actions",10);
                    }
                    break;
                case actions.walkBack:
                    if(anim.GetInteger("actions") != 11) {
                        PlayerStatus.playerIsDoing = "walkBack";
                        anim.SetInteger("actions",11);
                    }
                    break;
                case actions.strafeRight:
                    if(anim.GetInteger("actions") != 12) {
                        PlayerStatus.playerIsDoing = "walk";
                        anim.SetInteger("actions",12);
                    }
                    break;
                case actions.strafeLeft:
                    if(anim.GetInteger("actions") != 13) {
                        PlayerStatus.playerIsDoing = "walk";
                        anim.SetInteger("actions",13);
                    }
                    break;
                case actions.gunAim:
                    if (anim.GetInteger("actions") != 14){
                        PlayerStatus.playerIsDoing = "aim";
                        anim.SetInteger("actions", 14);
                    }
                    break;
                case actions.dead:
                    if (anim.GetInteger("actions") != 17 && anim.GetInteger("actions") != 2){
                        anim.SetInteger("actions", 2);
                    }
                    break;
                case actions.hitHard:
                    if (anim.GetInteger("actions") != 17 && anim.GetInteger("actions") != 18){
                        anim.SetInteger("actions", 17);
                    }
                    break;
            }
        }

        if(Cursor.visible) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
	}

    void OnAnimatorIK() {
        if (isDoing == actions.gunAim) {
            RaycastHit lookAtAim;
            Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward,out lookAtAim,9999f,1<<0);
            anim.SetLookAtPosition (lookAtAim.point);
            anim.SetLookAtWeight(1.0f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand,lookAtAim.point);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); 
        }
    }

    public void EndAction() {
        canShout = false;
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

    public void ContinueCombo() {
        if (!combo || !hudScript.UseStamina(stamContinueCombo)) {
            if(isDoing == actions.combo) {
                EndAction();
            }
        }
        else {
            if (atkForward) {
                transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
            }
        }
    }

	void OnTriggerEnter(Collider coll) {
        if(coll.name != "coll") {
            if(coll.gameObject.layer == LayerMask.NameToLayer("weapon") && hitbox.activeSelf && !hitDetect) {
                Vector3 posPlayer = transform.position + transform.up * 2f;
                if (coll.transform.name == "orb") {
                    float proximity = damageOrbCurve.Evaluate((coll.transform.position - transform.position).magnitude/maxDistanceOrbDamage);
                    if(proximity > 0.5f) {
                        Quaternion rot = Quaternion.LookRotation(new Vector3(coll.transform.position.x,transform.position.y,coll.transform.position.z) - transform.position);
                        rot *= Quaternion.Euler(0,180,0);
                        transform.rotation = rot;
                        StartCoroutine(Hit(true));
                        hitIsHard = true;
                    }
                    else {
                        StartCoroutine(Hit(false));
                        hitIsHard = false;
                    }
                    hudScript.DamageHP(proximity * setupValues.player_OrbMaxDamage);  
                    Instantiate(damageParticle,posPlayer-(posPlayer-coll.transform.position)/2f,Quaternion.identity);             
                }
                else if(coll.transform.name.Contains("bulletPf")) { 
                    hudScript.DamageHP(setupValues.player_DroneBullet);
                    StartCoroutine(Hit(false));
                    hitIsHard = false;
                    Instantiate(damageParticle,posPlayer-(posPlayer-coll.transform.position)/2f,Quaternion.identity);
                }
                else if(coll.transform.name == "colliderWeapon") { 
                    hudScript.DamageHP(setupValues.player_enemyAttack);
                    StartCoroutine(Hit(false));
                    hitIsHard = false;
                    Instantiate(damageParticle,posPlayer-(posPlayer-coll.transform.position)/2f,Quaternion.identity);
                }
                else if(coll.transform.name == "jumpAtk") { 
                    hudScript.DamageHP(setupValues.player_bossJumpAttack);
                    Quaternion rot = Quaternion.LookRotation(new Vector3(coll.transform.position.x,transform.position.y,coll.transform.position.z) - transform.position);
                    rot *= Quaternion.Euler(0,180,0);
                    transform.rotation = rot;
                    StartCoroutine(Hit(true));
                    hitIsHard = true;
                    Instantiate(damageParticle,posPlayer-(posPlayer-coll.transform.position)/2f,Quaternion.identity);
                }
                else if(coll.transform.name == "atk1") {
                    hudScript.DamageHP(setupValues.player_bossRightAttack);
                    Quaternion rot = Quaternion.LookRotation(new Vector3(coll.transform.position.x,transform.position.y,coll.transform.position.z) - transform.position);
                    rot *= Quaternion.Euler(0,180,0);
                    transform.rotation = rot;
                    StartCoroutine(Hit(true));
                    hitIsHard = true;
                    Instantiate(damageParticle,posPlayer-(posPlayer-coll.transform.position)/2f,Quaternion.identity);
                }
                else if(coll.transform.name == "atk") { 
                    hudScript.DamageHP(setupValues.player_bossLeftAttack);
                    StartCoroutine(Hit(false));
                    hitIsHard = false;
                    Instantiate(damageParticle,posPlayer-(posPlayer-coll.transform.position)/2f,Quaternion.identity);
                }
            }
        }
    }

    public void Dead() {
        hitDetect = true;
        PlayerStatus.playerIsDoing = "dead";
        if(hitIsHard) {
            anim.SetInteger("actions", 17);
        }
        else {
            anim.SetInteger("actions", 2);
            voiceSource.clip = audioClips[9];
            voiceSource.Play();
        }
        
        
        //GetComponent<Rigidbody>().isKinematic = true; 
        GetComponent<characterController>().enabled = false;
        //StartCoroutine(DeactiveCollider()); 
        
    }

    IEnumerator DeactiveCollider() {
        yield return new WaitForSeconds(1f);
        GetComponent<CapsuleCollider>().enabled = false;
    }

    IEnumerator Hit(bool hitHard) {
        hitDetect = true;
        
        if(isDoing == actions.gunAim) {
            cameraMain.GetComponent<cameraOrbitController>().AimCamera(false);
            hudScript.AimFeed(false);
            atkForward = false;
        }
        if (hitHard) {
            isDoing = actions.hitHard;
            doAction = actions.hitHard;
            AudioHitHard();
            yield return new WaitForSeconds(setupValues.hardHit);
            
        }
        else if(PlayerStatus.playerIsDoing != "dead"){
            isDoing = actions.hit;
            doAction = actions.hit;
            AudioHit();
            yield return new WaitForSeconds(setupValues.hit);
            
        }
        hitDetect = false;
    }

    public void HitHardStand() {
        if(PlayerStatus.playerIsDoing != "dead") {
            anim.SetInteger("actions", 18);
        }
    }

    public void AudioCombo() {
        if(canShout) {
            int rng = Random.Range(0,4);
            while (rng == notRepeatShout){
                rng = Random.Range(0,4);
            }
            voiceSource.clip = audioClips[rng];
            notRepeatShout = rng;
            voiceSource.Play();
        }
    }

    public void AudioSlash() {
        if(canShout) {
            voiceSource.clip = audioClips[4];
            voiceSource.Play();
        }
    }

    public void AudioHit(){
        int rng = Random.Range(0, 3);
        while (rng == notRepeatHit){
            rng = Random.Range(0, 3);
        }
        voiceSource.clip = audioClips[rng+5];
        voiceSource.Play();
        notRepeatHit = rng;
    }
    public void AudioHitHard(){
        voiceSource.clip = audioClips[8];
        voiceSource.Play();
    }

    public void Particles(int aux) {
        Instantiate(particleAttack, particlePositions[aux].position, particlePositions[aux].rotation);
    }
}
