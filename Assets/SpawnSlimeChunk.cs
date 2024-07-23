using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSlimeChunk : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public int animalsPerChunk = 5;
    public int chunkSize = 100;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < animalsPerChunk; i++)
        {
            SpawnAnimal();
        }

        void SpawnAnimal()
        {
            int randomIndex = Random.Range(0, animalPrefabs.Length);
            GameObject animal = animalPrefabs[randomIndex];

            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(0, chunkSize),
                100,
                transform.position.z + Random.Range(0, chunkSize)
            );

            Instantiate(animal, randomPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
