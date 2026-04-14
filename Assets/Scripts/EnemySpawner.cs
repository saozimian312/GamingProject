using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public GameObject normalEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject tankEnemyPrefab;

    public Transform spawnPoint;
    public Transform targetPoint;
    public TMP_Text waveText;

    public int totalWaves = 5;
    public int baseEnemiesPerWave = 3;
    public int enemiesIncreasePerWave = 1;
    public float timeBetweenEnemies = 1f;
    public float timeBetweenWaves = 2f;

    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool isSpawning = false;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.WinGame();
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
            GameObject enemyPrefabToSpawn = GetRandomEnemyPrefab();

            if (enemyPrefabToSpawn != null)
            {
                GameObject enemy = Instantiate(enemyPrefabToSpawn, spawnPoint.position, Quaternion.identity);

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
        int roll = Random.Range(0, 100);

        // 你可以改这里的概率
        if (roll < 50)
        {
            return normalEnemyPrefab; // 50%
        }
        else if (roll < 80)
        {
            return fastEnemyPrefab;   // 30%
        }
        else
        {
            return tankEnemyPrefab;   // 20%
        }
    }

    public void NotifyEnemyDestroyed()
    {
        activeEnemies--;

        if (activeEnemies <= 0 && !isSpawning)
        {
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