using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "UniverseData", menuName = "Procedural World Generation/Universe Data")]
public class Universe : ScriptableObject
{
    public string universeName;
    public Biome[] biomeList;
    public Structure[] structureList;
    public Enemy[] enemies;
    
    private int _enemyProbabilityPrecision = 100;
    private int[] _enemyProbabilities;
    
    public void SetupBiomes()
    {
        foreach (var biome in biomeList)
        {
            biome.PreProcess();
        }
    }
    
    public Biome GetBiomeByParameters(float temperature, float humidity)
    {
        foreach (var biome in biomeList)
        {
            if (biome.Validate(temperature, humidity))
                return biome;
        }

        // if (defaultBiome != null)
        //     return defaultBiome;
        // else 
        if (biomeList.Length > 0)
            return biomeList[0];

        return null;
    }

    public void EnemyProbabilitiesPreProcess()
    {
        float[] normalizedEnemyProbabilities = new float[enemies.Length];
        float nomalizeFactor = 0;

        foreach (var enemy in enemies)
        {
            nomalizeFactor += enemy.pickRate;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            normalizedEnemyProbabilities[i] = enemies[i].pickRate / nomalizeFactor;
        }
        
        _enemyProbabilities = new int[_enemyProbabilityPrecision];

        int currentEnemyIndex = 0;
        int counter = 0;
        
        for (int i = 0; i < _enemyProbabilityPrecision; i++)
        {
            if (currentEnemyIndex < enemies.Length && counter > normalizedEnemyProbabilities[currentEnemyIndex] * _enemyProbabilityPrecision)
            {
                counter = 0;
                currentEnemyIndex++;
            }
            
            _enemyProbabilities[i] = currentEnemyIndex;
            counter++;
        }
    }
    
    public Enemy GetEnemy()
    {
        Random rand = new Random();
        int randomValue = rand.Next(0, _enemyProbabilityPrecision);

        if (enemies.Length > 0)
        {
            return enemies[_enemyProbabilities[randomValue]];
        }

        return null;
    }
    
    [System.Serializable]
    public class Structure
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float pickRate;
    }
    
    [System.Serializable]
    public class Enemy
    {
        public string name;
        public GameObject prefab;
        [Range(0f, 1f)]
        public float pickRate;
        public bool isHorde;
        public int hordeSize;
    }
}