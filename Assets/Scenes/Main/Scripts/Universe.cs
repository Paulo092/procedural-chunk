using UnityEngine;

[CreateAssetMenu(fileName = "UniverseData", menuName = "Procedural World Generation/Universe Data")]
public class Universe : ScriptableObject
{
    
    public string universeName;
    public Biome defaultBiome;
    public Biome[] biomeList;

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
}