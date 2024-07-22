using UnityEngine;
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

    [Header("Terrain Parameters")] 
    [Range(0f, 1f)]
    public float smoothness;

    [Header("Population")] 
    [Range(0f, 1f)]
    public float treeDensity;
    [Range(0f, 1f)]
    public float grassDensity;
    [Space]
    public BiomeTree[] biomeTrees;
    
    public Texture2D[] grass;
    public BiomeTexture[] biomeTextures;

    private int _probabilityPrecision = 100;
    private int[] _treeProbabilities;
    
    public void PreProcess()
    {
        float[] normalizedTrees = new float[biomeTrees.Length];
        float nomalizeFactor = 0;

        foreach (var tree in biomeTrees)
        {
            nomalizeFactor += tree.pickRate;
        }

        for (int i = 0; i < biomeTrees.Length; i++)
        {
            normalizedTrees[i] = biomeTrees[i].pickRate / nomalizeFactor;
        }
        
        _treeProbabilities = new int[_probabilityPrecision];

        int currentTreeIndex = 0;
        int counter = 0;
        
        // string finalResult = "";
        
        for (int i = 0; i < _probabilityPrecision; i++)
        {
            if (currentTreeIndex < biomeTrees.Length && counter > normalizedTrees[currentTreeIndex] * _probabilityPrecision)
            {
                counter = 0;
                currentTreeIndex++;
            }
            
            _treeProbabilities[i] = currentTreeIndex;
            counter++;

            // finalResult += "- " + i + ": " + currentTreeIndex + " -";
        }

        // Debug.Log("Final: " + finalResult);
    }

    public BiomeTree GetTree()
    {
        Random rand = new Random();
        int randomValue = rand.Next(0, _probabilityPrecision);

        if (biomeTrees.Length > 0)
        {
            return biomeTrees[_treeProbabilities[randomValue]];
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
}