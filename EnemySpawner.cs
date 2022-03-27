using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabA;
    [SerializeField] private GameObject enemyPrefabB;
    [SerializeField] private GameObject enemyPrefabC;
    [SerializeField] private int nEnemiesInWave = 0;
    [SerializeField] private float spawnEnemyInWaveWaitTime = 0.5f;
    [SerializeField] private bool isWaveSpawning = false;
    [SerializeField] private float spawnTimer = 5f;
    [SerializeField] private float timer = 5f;

    private int waveNumber = 0;
    private int spawnType; // 0 - ZomA, 1 - ZomB, 2 - ZomC
    private int currentEnemiesAlive = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0 && currentEnemiesAlive <= 0)
        {
            waveNumber++;
            currentEnemiesAlive = nEnemiesInWave;
            StartCoroutine(SpawnWave());
            timer = spawnTimer;
        }
        else if(!isWaveSpawning && currentEnemiesAlive <= 0)
            timer -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        isWaveSpawning = true;
        for (int i = 0; i < nEnemiesInWave; i++)
        {
            SpawnEnemy(3);
            yield return new WaitForSeconds(spawnEnemyInWaveWaitTime);
        }
        if(nEnemiesInWave < 100)
        nEnemiesInWave += 20; // add 20 enemies per wave
        isWaveSpawning = false;
    }

    private void SpawnEnemy(int highrandom)
    {
        spawnType = Random.Range(0, highrandom);
        switch (spawnType)
        {
            case 0:
                Instantiate(enemyPrefabA, transform);
                break;
            case 1:
                Instantiate(enemyPrefabB, transform);
                break;
            case 2:
                Instantiate(enemyPrefabC, transform);
                break;
            default:
                break;
        }
    }

    public int GetNEnemiesInWave()
    {
        return nEnemiesInWave;
    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public int GetEnemiesAlive()
    {
        return currentEnemiesAlive;
    }

    public void EnemyKilled()
    {
        currentEnemiesAlive--;
    }
}
