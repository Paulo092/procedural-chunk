using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[System.Serializable]
public class Biome
{
    public string biomeName;
    [Header("Biome Parameters")]
    [MinMaxSlider(0,1)]
    public Vector2 temperature = new Vector2(0f,0f);    
    [MinMaxSlider(0,1)]
    public Vector2 humidity = new Vector2(0f,0f);
    public BiomeTexture[] biomeTextures;
    
    [Header("Terrain Parameters")] 
    [Range(0f, 1f)]
    public float smoothness;

    [Header("Population")] 
    [Range(0f, 1f)]
    public float treeDensity;
    public BiomeTree[] biomeTrees;
    
    [Space]
    [Range(0f, 1f)]
    public float grassDensity;
    public Texture2D[] grass;

    [Space]
    [Range(0f, 1f)]
    public float structureDensity;
    public BiomeStructure[] biomeStructures;
    
    private int _treeProbabilityPrecision = 100;
    private int _structureProbabilityPrecision = 100;
    private int[] _treeProbabilities;
    private int[] _structureProbabilities;

    // [FormerlySerializedAs("GemStructurePrefab")] [Header("Biome Gem")]
    // public GameObject gemStructurePrefab;
    // public Gem biomeGem;
    // [Range(0f, 1f)] 
    // public float gemProbability;
    
    // [Space]
    // public GameObject miniGemStructurePrefab;
    // public Gem miniBiomeGem;
    // [Range(0f, 1f)] 
    // public float miniGemProbability;
    
    public void PreProcess()
    {
        TreePreProcess();
        StructurePreProcess();
    }

    public void TreePreProcess()
    {
        float[] normalizedTrees = new float[biomeTrees.Length];
        float nomalizeFactor = 0;

        foreach (var tree in biomeTrees)
            nomalizeFactor += tree.pickRate;

        for (int i = 0; i < biomeTrees.Length; i++)
            normalizedTrees[i] = biomeTrees[i].pickRate / nomalizeFactor;
        
        _treeProbabilities = new int[_treeProbabilityPrecision];

        int currentTreeIndex = 0;
        int counter = 0;
        
        for (int i = 0; i < _treeProbabilityPrecision; i++)
        {
            if (currentTreeIndex < biomeTrees.Length && counter > normalizedTrees[currentTreeIndex] * _treeProbabilityPrecision)
            {
                counter = 0;
                currentTreeIndex++;
            }
            
            _treeProbabilities[i] = currentTreeIndex;
            counter++;

        }
    }

    public void StructurePreProcess()
    {
        float[] normalizedStructures = new float[biomeStructures.Length];
        float nomalizeFactor = 0;

        foreach (var structure in biomeStructures)
            nomalizeFactor += structure.pickRate;

        for (int i = 0; i < biomeStructures.Length; i++)
            normalizedStructures[i] = biomeStructures[i].pickRate / nomalizeFactor;
        
        _structureProbabilities = new int[_structureProbabilityPrecision];

        int currentStructureIndex = 0;
        int counter = 0;
        
        for (int i = 0; i < _structureProbabilityPrecision; i++)
        {
            if (currentStructureIndex < biomeStructures.Length && counter > normalizedStructures[currentStructureIndex] * _structureProbabilityPrecision)
            {
                counter = 0;
                currentStructureIndex++;
            }
            
            _structureProbabilities[i] = currentStructureIndex;
            counter++;
        }
    }

    public BiomeTree GetTree()
    {
        Random rand = new Random();
        int randomValue = rand.Next(0, _treeProbabilityPrecision);

        if (biomeTrees.Length > 0)
        {
            return biomeTrees[_treeProbabilities[randomValue]];
        }

        return null;
    }
    
    public BiomeStructure GetStructure()
    {
        Random rand = new Random();
        int randomValue = rand.Next(0, _structureProbabilityPrecision);

        if (biomeStructures.Length > 0)
        {
            return biomeStructures[_structureProbabilities[randomValue]];
        }

        return null;
    }
    
    public bool Validate(float temperatureToEvaluate, float humidityToEvaluate)
    {
        return (
            (temperatureToEvaluate >= this.temperature.x && temperatureToEvaluate <= this.temperature.y) &&
            (humidityToEvaluate >= this.humidity.x && humidityToEvaluate <= this.humidity.y)
        );
    }

    [System.Serializable]
    public class BiomeTexture
    {
        [HideInInspector]
        public int layer = 0;
        public Texture2D texture;
        [MinMaxSlider(0,1)]
        public Vector2 altitude = new Vector2(0f,0f);
    }

    [System.Serializable]
    public class BiomeTree
    {
        public GameObject treePrefab;
        [Range(0f, 1f)] 
        public float pickRate;
    }
    
    [System.Serializable]
    public class BiomeStructure
    {
        public GameObject structurePrefab;
        [Range(0f, 1f)] 
        public float pickRate;
    }
}