using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    public int totalWaves = 3;
    public int baseEnemiesPerWave = 3;
    public int enemiesIncreasePerWave = 1;

    [Range(0, 100)] public int normalChance = 50;
    [Range(0, 100)] public int fastChance = 30;
    [Range(0, 100)] public int tankChance = 20;
}