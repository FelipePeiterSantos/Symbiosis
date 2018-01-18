using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : MonoBehaviour {

    public GameObject ultiOk;
    public GameObject ultiCD;

    public RawImage hp_bar;
    public float max_hp, min_hp;
    float range_hp;
    public Text hp_text;
    public RawImage stam_bar;
    public float max_stam, min_stam;
    float range_stam;
    public Text stam_text;
    public RawImage ulti_bar;
    public float max_ulti, min_ulti;
    float range_ulti;
    public Text ulti_text;
    public int ulti_cooldown_seconds;
    public GameObject aim;
    public GameObject[] commands;
    public RawImage bloodScreen;
    public Color bloodScreenMax;
    public GameObject noStamina;
    public characterController playerScript;
    public GameObject youDied;
    public GameObject[] deactiveInAwake;

    static bool showCommands = false;
    
    float HP = 1000;
    float compareHP;
    float stam = 100;
    float compareStam;
    float ulti = 0;

    float compareUlti;

    float regen_HP_cd;
    float regen_stam_cd;
    float ulti_cd;

    float feedNoStam;

    void Awake() {
        range_hp = (max_hp - min_hp)/1000;
        range_stam = (max_stam - min_stam)/100;
        range_ulti = (max_ulti - min_ulti)/100;
        regen_HP_cd = 0;
        regen_stam_cd = 0;
        ulti_cd = 0;
        ultiOk.SetActive(false);
        ultiCD.SetActive(true);

        bloodScreen.color = new Color(bloodScreen.color.r, bloodScreen.color.g, bloodScreen.color.b,0);
        for (int i = 0; i < commands.Length; i++){
            commands[i].SetActive(false);
        }
        noStamina.SetActive(false);

        feedNoStam = 0;
        youDied.SetActive(false);
        for (int i = 0; i < deactiveInAwake.Length; i++){
            deactiveInAwake[i].SetActive(false);
        }
    }

    void Start() {
        ulti_cooldown_seconds = setupValues.energyOrb_cooldown;
        ulti_cd = 100f/(100f/ulti_cooldown_seconds); 
    }   

    void Update() {
        if(Input.GetKeyDown(KeyCode.Joystick1Button7)) {
            commands[0].SetActive(false);
            commands[1].SetActive(!commands[1].activeSelf);
        }
        else if(Input.GetKeyDown(KeyCode.Return)) {
            commands[0].SetActive(!commands[0].activeSelf);
            commands[1].SetActive(false);
        }

        if(HP < 1000) {
            regen_HP_cd += Time.deltaTime;
            if(regen_HP_cd > 5f) {
                HP += 25f * Time.deltaTime;
            }
        }
        else if (HP > 1000) {
            HP = 1000;
        }

        if(stam < 100) {
            regen_stam_cd += Time.deltaTime;
            if(regen_stam_cd > 1f) {
                stam += 15f * Time.deltaTime;
            }
        }
        else if (stam > 100) {
            stam = 100;
        }

        if(ulti < 100) {
            ulti += (100f/ulti_cooldown_seconds) * Time.deltaTime;
            ulti_cd -= Time.deltaTime;
        }
        else if (ulti > 100) {
            ulti = 100;
        }

        if (feedNoStam > 0) {
            feedNoStam -= Time.deltaTime;
            if(!noStamina.activeSelf)
                noStamina.SetActive(true);
        }
        else if (noStamina.activeSelf) {
            noStamina.SetActive(false);
        }
    }

    void LateUpdate() {
        if(HP != compareHP) {
            RefreshHPBar(0);
            compareHP = HP;
        }
        if(stam != compareStam) {
            RefreshHPBar(1);
            compareStam = stam;
        }
        if(ulti != compareUlti) {
            RefreshHPBar(2);
            compareUlti = ulti;
        }

        if(ulti == 100 && !ultiOk.activeSelf) {
            ultiOk.SetActive(true);
            ultiCD.SetActive(false);
        }
    }

    public void DamageHP(float aux) {
        regen_HP_cd = 0;
        if(HP > 0) {
            HP -= aux;
        }

        if(HP <= 0) {
            playerScript.Dead();
            youDied.SetActive(true);
            HP = 0;
            RefreshHPBar(0);
            transform.GetComponent<HUDManager>().enabled = false;
        }
        StartCoroutine("HitFeed");
    }
    public bool UseStamina(float aux) {
        if(stam > 0) {
            regen_stam_cd = 0;
            stam -= aux;
            return true;
        }
        else if (aux > stam) {
            feedNoStam = 1f;
        }
        return false;
    }

    public bool UseUlti() {
        if(ulti == 100) {
            ulti = 0;
            ulti_cd = 100 / (100f/ulti_cooldown_seconds);
            ultiCD.SetActive(true);
            ultiOk.SetActive(false);
            return true;
        }
        return false;
    }

    void RefreshHPBar(int aux) {
        if(aux == 0) {
            float displayConvert = (HP * range_hp)+min_hp;
            Vector3 auxPos = hp_bar.rectTransform.localPosition;
            auxPos.x = displayConvert;
            hp_bar.rectTransform.localPosition = auxPos;
            hp_text.text = Mathf.Round(HP).ToString();
        }
        else if(aux == 1) {
            float displayConvert = (stam * range_stam)+min_stam;
            Vector3 auxPos = stam_bar.rectTransform.localPosition;
            auxPos.x = displayConvert;
            stam_bar.rectTransform.localPosition = auxPos;
            stam_text.text = Mathf.Round(Mathf.Clamp(stam,0f,100f)).ToString();
        }
        else if(aux == 2) {
            float displayConvert = (ulti * range_ulti)+min_ulti;
            Vector3 auxPos = ulti_bar.rectTransform.localPosition;
            auxPos.y = displayConvert;
            ulti_bar.rectTransform.localPosition = auxPos;
            ulti_text.text = Mathf.Round(ulti_cd).ToString();
        }
    }

    public void AimFeed(bool aux) {
        aim.SetActive(aux);
    }

    IEnumerator HitFeed() {
        regen_stam_cd = 0;
        bloodScreen.color = bloodScreenMax;
        while(bloodScreen.color.a > 0) {
            bloodScreen.color = new Color(1, 0, 0, bloodScreen.color.a -Time.deltaTime);
            yield return false;
        }
        yield return true;
        bloodScreen.color = new Color(bloodScreen.color.r, bloodScreen.color.g, bloodScreen.color.b,0);
    }
}
