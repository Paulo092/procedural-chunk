using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TowerGemCollect : MonoBehaviour
{
    public GameObject orbitCenter;
    public Gem[] allGemsPrefabs;
    // private List<GameObject> allGemsSpawned = new();
    public List<TowerGemInfo> towerGemInventory = new();

    private void Start()
    {
        StartCoroutine(nameof(SpawnPlaceholderGem));
    }

    IEnumerator SpawnPlaceholderGem()
    {
        foreach (var gem in allGemsPrefabs)
        {
            GameObject followGem = Instantiate(gem.prefab, this.transform.position, UnityEngine.Quaternion.identity);
            followGem.transform.localScale = new Vector3(1f, 1f, 1f);

            // followGem.GetComponent<Gem>().isNatural = false;
            
            GemOrbit orbit = followGem.AddComponent<GemOrbit>();
            orbit.heightOffset = new Vector3(0f, 5f, 0f);
            orbit.target = orbitCenter.transform;
            orbit.orbitDistance = 15;

            if (gem.notFoundedMaterial != null)
            {
                followGem.GetComponent<Renderer>().material = gem.notFoundedMaterial;
            }

            TowerGemInfo gemInfo = new();
            gemInfo.Prefab = followGem;
            gemInfo.Gem = gem;
            gemInfo.IsReturned = false;
            
            towerGemInventory.Add(gemInfo);
            // allGemsSpawned.Add(followGem);
            
            yield return new WaitForSeconds((360f / 30f) / allGemsPrefabs.Length);
        }
    }
    
    // private IEnumerator VerifyGems(List<Gem> collectedGems)
    // {
    //     foreach (var collectedGem in collectedGems)
    //     {
    //         foreach (var gem in allGemsSpawned)
    //         {
    //             if (collectedGem.gemIdentifier == gem.GetComponent<Gem>().gemIdentifier)
    //             {
    //                 ActivateGem(gem);
    //                 // Destroy(collectedGem.gameObject);
    //                 yield return new WaitForSeconds(2f);
    //             }
    //         }
    //     }
    // }

    private void ActivateGem(Gem gem)
    {
        gem.GetComponent<Renderer>().material = gem.GetComponent<Gem>().normalMaterial;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(nameof(UpdateGemStatus));

    }

    private IEnumerator UpdateGemStatus()
    {
        GemHandler instance = GemHandler.GetInstance();

        Debug.Log(">>> : " + instance.allGems.Length + " - " + instance.numberOfCollectedGems);
        
        if (instance.allGems.Length == instance.numberOfCollectedGems)
        {
            Debug.Log("Ganhou!");
            yield return null;
        }
        
        foreach (var gem in GemHandler.GetInstance().allGems)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("Gem " + gem.gem.gemIdentifier + " - isC: " + gem.isCollected);
            gem.isReturned = true;
        }
    }
    
    public class TowerGemInfo
    {
        public Gem Gem;
        public GameObject Prefab;
        public Boolean IsReturned;
    }
}
