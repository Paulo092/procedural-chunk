using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TowerGemCollect : MonoBehaviour
{

    public GameObject orbitCenter;
    public Gem[] allGemsPrefabs;
    private List<GameObject> allGemsSpawned = new();

    private void Start()
    {
        StartCoroutine(nameof(SpawnPlaceholderGem));
    }

    IEnumerator SpawnPlaceholderGem()
    {
        Random rand = new Random();
        foreach (var gem in allGemsPrefabs)
        {
            GameObject followGem = Instantiate(gem.prefab, this.transform.position, UnityEngine.Quaternion.identity);
            followGem.transform.localScale = new Vector3(1f, 1f, 1f);

            followGem.GetComponent<Gem>().isNatural = false;
            
            PlayerGemOrbit orbit = followGem.AddComponent<PlayerGemOrbit>();
            orbit.heightOffset = new Vector3(0f, 5f, 0f);
            orbit.player = orbitCenter.transform;
            orbit.orbitDistance = 15;

            if (gem.notFoundedMaterial != null)
            {
                followGem.GetComponent<Renderer>().material = gem.notFoundedMaterial;
            }
            
            allGemsSpawned.Add(followGem);
            
            yield return new WaitForSeconds((360f / 30f) / allGemsPrefabs.Length);
        }
    }

    private IEnumerator VerifyGems(List<Gem> collectedGems)
    {
        foreach (var collectedGem in collectedGems)
        {
            foreach (var gem in allGemsSpawned)
            {
                if (collectedGem.gemIdentifier == gem.GetComponent<Gem>().gemIdentifier)
                {
                    ActivateGem(gem);
                    // Destroy(collectedGem.gameObject);
                    yield return new WaitForSeconds(2f);
                }
            }
        }
    }

    private void ActivateGem(GameObject gem)
    {
        gem.GetComponent<Renderer>().material = gem.GetComponent<Gem>().normalMaterial;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Random rand = new Random();
        GemBag gemBag = other.GetComponent<GemBag>();

        if (gemBag != null && GemBag.collectedGems.Count > 0)
        {
            Debug.Log("Deu certo");
            StartCoroutine(nameof(VerifyGems), GemBag.collectedGems);
        }
    }
}
