using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public GameObject chunkPrefab;
    public int chunkSize = 99;
    public int terrainWidth = 100;
    public int terrainDepth = 100;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < terrainWidth; x += chunkSize)
        {
            for (int z = 0; z < terrainDepth; z += chunkSize)
            {
                Vector3 chunkPosition = new Vector3(x, 50, z);
                // Vector3 chunkPosition = new Vector3(0, 0, 0);

                Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
            }
        }

        // Vector3 chunkPosition = new Vector3(0, 100, 0);
        // Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
