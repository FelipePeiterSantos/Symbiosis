using UnityEngine;
using System.Collections;

public class spawnerController : MonoBehaviour {

    public GameObject[] doors;
    public Transform[] spawnPoints;
    public Transform bossPoint;
    public GameObject enemy_pf;
    public GameObject drone_pf;
    public GameObject boss_pf;
    public AudioClip randomMusic;
    AudioSource musicSource;
    public int spawner;
    int sameTime;
    int quantityEnemies;
    float randomEnemy = 0.8f;
    bool start;

    void Start() {
        musicSource = GetComponent<AudioSource>();
        switch(spawner) {
            case 1:
                sameTime = setupValues.spawn1_sameTime;
                quantityEnemies = setupValues.spawn1_quantityEnemies;
                break;
            case 2:
                sameTime = setupValues.spawn2_sameTime;
                quantityEnemies = setupValues.spawn2_quantityEnemies;
                break;
            case 3:
                sameTime = setupValues.spawn3_sameTime;
                quantityEnemies = setupValues.spawn3_quantityEnemies;
                break;
            case 4:
                sameTime = setupValues.spawn4_sameTime;
                break;
        }
    }

    public void SpawnMore() {
        int enemiesAlive = 0;
        bool bossActive = false;
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("Enemy")) {
            if(item.name.Contains("inimigos") || item.name.Contains("Drone")) {
                enemiesAlive++;
            }
            if(item.name.Contains("boss")) {
                bossActive = true;
            }
        }
        if(bossActive) {
            quantityEnemies = 1;
        }

        if(quantityEnemies > 0 && enemiesAlive < sameTime) {
            StartCoroutine(SpawnRandom(Random.Range(0, spawnPoints.Length)));
        }
        else if(enemiesAlive <= 0) {
            doors[0].transform.parent = null;
            StartCoroutine("StopMusic");
        }
    }

	void OnTriggerEnter(Collider coll) {
        if (coll.transform.CompareTag("Player") && !start) {
            for (int i = 0; i < doors.Length; i++){
                doors[i].SetActive(true);
            }
            for (int i = 0; i < sameTime; i++){
                StartCoroutine(SpawnRandom(i));
            }
            start = true;
            if(spawner == 4) {
                Instantiate(boss_pf,bossPoint.position,Quaternion.identity);
            }
            InvokeRepeating("SpawnMore",1f,1f);
            StartCoroutine("StartMusic");
        }
    }

    IEnumerator SpawnRandom(int spawnPoint) {
        spawnPoints[spawnPoint].GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        float rng = Random.Range(0f,1f);
        if(rng > randomEnemy) {
            Instantiate(drone_pf,spawnPoints[spawnPoint].position,Quaternion.identity);
            randomEnemy += 0.2f;
        }
        else {
            Instantiate(enemy_pf,spawnPoints[spawnPoint].position,Quaternion.identity);
            randomEnemy -= 0.1f;
        }
        quantityEnemies--;
        yield return new WaitForSeconds(1.5f);
        spawnPoints[spawnPoint].GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator StartMusic() {
        if(Random.Range(0f,1000f) < 1f) {
            musicSource.clip = randomMusic;
        }
        musicSource.Play();
        musicSource.volume = 0;
        yield return true;
        while(musicSource.volume < 1f) {
            musicSource.volume += Time.deltaTime*0.5f;
            yield return false;
        }
        yield return true;
    }

    IEnumerator StopMusic() {
        while(musicSource.volume > 0f) {
            musicSource.volume -= Time.deltaTime*0.25f;
            yield return false;
        }
        yield return true;
        Destroy(this.gameObject);
    }
}
