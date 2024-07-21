using UnityEngine;

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
    public BiomeTree[] biomeTrees;
    
    public Texture2D[] grass;
    public BiomeTexture[] biomeTextures;

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
        public float density;
    }
}