using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyWave[] waves;
    private int activeWave = 0;
    private GameObject[] enemiesToSpawn;

    [SerializeField]
    private int maxWaveTime = 60; // seconds for each wave

    [SerializeField]
    private int maxEnemies = 100;

    [SerializeField]
    private float levelModifier;

    [SerializeField]
    private PlayerState player;

    List<GameObject> activeEnemies;
    private float waveTimer;
    private float spawnRate;
    private float spawnTimer;
    private Camera cam;

    int spawnRateModifier = 1;


    // Start is called before the first frame update
    void Start()
    {
        activeEnemies = new List<GameObject>();
        cam = Camera.main;  
        NextWave(); 
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        waveTimer+= Time.deltaTime;

        if(spawnTimer > spawnRate) {
            SpawnEnemy();
            spawnTimer = 0;
        }

        if(waveTimer > maxWaveTime) {
            NextWave();
            waveTimer = 0;
        }

        //Debug.Log(activeEnemies.Count);
    }

    void NextWave() {
        enemiesToSpawn = waves[activeWave].enemies;
        spawnRate = 1f / (waves[activeWave].spawnRate * spawnRateModifier);
        activeWave++;

        if(activeWave >= waves.Length) {
            activeWave = 0;
            spawnRateModifier++;
        }
    }

    void SpawnEnemy() {
        // Gets rid of all dead enemied from the list
        activeEnemies.RemoveAll(s => s == null);
        if(activeEnemies.Count >= maxEnemies)
            return;

        int spawnSide = Random.Range(0, 4);
        float x, y;
        Vector2 spawnLoc;

        if(spawnSide == 0) {
            x = -0.1f;
            y = Random.Range(-0.1f, 1.1f);
        }
        else if(spawnSide == 1) {
            x = Random.Range(-0.1f, 1.1f);
            y = 1.1f;
        }
        else if(spawnSide == 2) {
            x = Random.Range(-0.1f, 1.1f);
            y = -0.1f;
        }
        else {
            x = 1.1f;
            y = Random.Range(-0.1f, 1.1f);
        }
        spawnLoc = cam.ViewportToWorldPoint(new Vector3(x, y, 10));

        GameObject enemy = Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)], spawnLoc, Quaternion.identity);
        EnemyState state = enemy.GetComponentInChildren<EnemyState>();
        //Debug.Log(state.hp);
        state.hp = (int)((float)state.hp * (1 + (player.level * levelModifier)));
        //Debug.Log(state.hp);

        activeEnemies.Add(enemy);
    }
}
