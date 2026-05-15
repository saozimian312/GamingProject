using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject tankEnemyPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;
    public Transform targetPoint;
    public TMP_Text waveText;

    [Header("Timing")]
    public float timeBetweenEnemies = 1f;
    public float timeBetweenWaves = 2f;

    [Header("Spawn Offset")]
    public float spawnRandomX = 0.4f;
    public float spawnRandomZ = 0.2f;

    private int totalWaves = 3;
    private int baseEnemiesPerWave = 3;
    private int enemiesIncreasePerWave = 1;

    private int normalChance = 50;
    private int fastChance = 30;
    private int tankChance = 20;

    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool isSpawning = false;

    public void ApplyLevelConfig(LevelConfig config)
    {
        if (config == null) return;

        totalWaves = config.totalWaves;
        baseEnemiesPerWave = config.baseEnemiesPerWave;
        enemiesIncreasePerWave = config.enemiesIncreasePerWave;

        normalChance = config.normalChance;
        fastChance = config.fastChance;
        tankChance = config.tankChance;
    }

    public void StartLevel()
    {
        StopAllCoroutines();

        currentWave = 0;
        activeEnemies = 0;
        isSpawning = false;

        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        Debug.Log("StartNextWave called. currentWave = " + currentWave + ", totalWaves = " + totalWaves);

        if (currentWave >= totalWaves)
        {
            Debug.Log("All waves finished, calling OnLevelCleared");

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnLevelCleared();
            }
            yield break;
        }

        currentWave++;
        UpdateWaveText();

        yield return new WaitForSeconds(timeBetweenWaves);

        isSpawning = true;

        int enemyCount = baseEnemiesPerWave + (currentWave - 1) * enemiesIncreasePerWave;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject prefabToSpawn = GetRandomEnemyPrefab();

            if (prefabToSpawn != null && spawnPoint != null)
            {
                Vector3 spawnOffset = new Vector3(
                    Random.Range(-spawnRandomX, spawnRandomX),
                    0f,
                    Random.Range(-spawnRandomZ, spawnRandomZ)
                );

                GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position + spawnOffset, Quaternion.identity);

                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.targetPoint = targetPoint;
                    controller.spawner = this;
                }

                activeEnemies++;
            }

            yield return new WaitForSeconds(timeBetweenEnemies);
        }

        isSpawning = false;
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int totalChance = normalChance + fastChance + tankChance;
        int roll = Random.Range(0, totalChance);

        if (roll < normalChance)
        {
            return normalEnemyPrefab;
        }

        roll -= normalChance;
        if (roll < fastChance)
        {
            return fastEnemyPrefab;
        }

        return tankEnemyPrefab;
    }

    public void NotifyEnemyDestroyed()
    {
        activeEnemies--;
        Debug.Log("Enemy removed. activeEnemies = " + activeEnemies);

        if (activeEnemies <= 0 && !isSpawning)
        {
            Debug.Log("All enemies cleared, starting next wave");
            StartCoroutine(StartNextWave());
        }
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + currentWave + "/" + totalWaves;
        }
    }
}