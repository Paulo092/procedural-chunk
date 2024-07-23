using UnityEngine;

[CreateAssetMenu(fileName = "UniverseData", menuName = "Procedural World Generation/Universe Data")]
public class Universe : ScriptableObject
{
    public string universeName;
    public Biome[] biomeList;
    public Structure[] structureList;
    public Enemy[] enemies;
    
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