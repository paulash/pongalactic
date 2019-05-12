using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Wave
{
    public GameObject[] enemies;
    public GameObject rewardPrefab;
    public float delayBetweenSpawns;
}

public delegate void OnWaveSpawn(int index, int total, Wave wave);
public delegate void OnWavesComplete();

public class LevelManager : MonoBehaviour
{
    public Wave[] waves;

    public OnWaveSpawn onWaveSpawn;
    public OnWavesComplete onWavesComplete;
    public string nextLevel = "MainMenu";

    int waveIndex = 0;
    int enemiesKilled = 0;

    public float delayTillWaves = 5;
    float waveTimeout = 0;
    float spawnTimeout = 0;
    int enemyIndex = 0;

    public AudioSource mainLoop { get; private set; }

    void Awake()
    {
        mainLoop = GetComponent<AudioSource>();
    }

    private void Start()
    {
        waveTimeout = Time.time + delayTillWaves;
    }

    private void Update()
    {
        if (waveIndex >= waves.Length)
            return;

        if (waveTimeout > Time.time)
            return;

        if (spawnTimeout < Time.time && enemyIndex < waves[waveIndex].enemies.Length)
        {
            if (enemyIndex == 0)
            {
                if (onWaveSpawn != null)
                    onWaveSpawn(waveIndex, waves.Length, waves[waveIndex]);
            }

            GameObject enemyGO = GameObject.Instantiate(waves[waveIndex].enemies[enemyIndex]);
            enemyGO.GetComponent<Ship>().onDeath = OnEnemyDeath;

            enemyIndex++;
            spawnTimeout = Time.time + waves[waveIndex].delayBetweenSpawns;
        }
    }

    void OnEnemyDeath(Ship enemy)
    {
        enemiesKilled++;

        // wave completed
        if (enemiesKilled == waves[waveIndex].enemies.Length)
        {
            if (waves[waveIndex].rewardPrefab != null)
                GameObject.Instantiate(waves[waveIndex].rewardPrefab, enemy.transform.position, Quaternion.identity);

            waveIndex++;

            enemiesKilled = 0;
            enemyIndex = 0;

            waveTimeout = delayTillWaves + Time.time;

            if (waveIndex == waves.Length)
            {
                mainLoop.Stop();
                if (onWavesComplete != null)
                    onWavesComplete();
            }
        }
    }
}
