using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TowerGemCollect : MonoBehaviour
{
    public GameObject orbitCenter;
    public List<GameObject> spawnedGems = new();
    public Gem[] allGemsPrefabs;
    // private List<GameObject> allGemsSpawned = new();
    public List<TowerGemInfo> towerGemInventory = new();

    private void Start()
    {
        StartCoroutine(nameof(SpawnPlaceholderGem));
    }

    IEnumerator SpawnPlaceholderGem()
    {
        foreach (GemHandler.GemInfo gemInfo in GemHandler.GetInstance().allGems)
        {
            GameObject orbitalGemGameObject = Instantiate(gemInfo.gem.prefab, this.transform.position, Quaternion.identity);
            orbitalGemGameObject.SetActive(false);
            orbitalGemGameObject.transform.localScale = new Vector3(3f, 3f, 3f);
        
            GemOrbit gemOrbit = orbitalGemGameObject.AddComponent<GemOrbit>();
            gemOrbit.target = orbitCenter.transform;
            gemOrbit.orbitDistance = 20;

            Gem orbitalGem = orbitalGemGameObject.AddComponent<Gem>();
            gemInfo.gem.CloneInto(orbitalGem);
            orbitalGem.isNatural = false;

            Renderer orbitalGemRender = orbitalGemGameObject.GetComponent<Renderer>();
            orbitalGemRender.material = gemInfo.gem.notFoundedMaterial;
            
            orbitalGemGameObject.SetActive(true);
            
            spawnedGems.Add(orbitalGemGameObject);
            
            yield return new WaitForSeconds((360f / 30f) / GemHandler.GetInstance().allGems.Length);
        }
    }
    // foreach (var gem in allGemsPrefabs)
    // {
    //     GameObject followGem = Instantiate(gem.prefab, this.transform.position, UnityEngine.Quaternion.identity);
    //     followGem.transform.localScale = new Vector3(1f, 1f, 1f);
    //
    //     // followGem.GetComponent<Gem>().isNatural = false;
    //         
    //     GemOrbit orbit = followGem.AddComponent<GemOrbit>();
    //     orbit.heightOffset = new Vector3(0f, 5f, 0f);
    //     orbit.target = orbitCenter.transform;
    //     orbit.orbitDistance = 15;
    //
    //     if (gem.notFoundedMaterial != null)
    //     {
    //         followGem.GetComponent<Renderer>().material = gem.notFoundedMaterial;
    //     }
    //
    //     TowerGemInfo gemInfo = new();
    //     gemInfo.Prefab = followGem;
    //     gemInfo.Gem = gem;
    //     gemInfo.IsReturned = false;
    //         
    //     towerGemInventory.Add(gemInfo);
    //     // allGemsSpawned.Add(followGem);
    //         
    //     yield return new WaitForSeconds((360f / 30f) / allGemsPrefabs.Length);
    // }
    
    
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
        PlayerOrbit playerOrbit = other.GetComponent<PlayerOrbit>();

        if (playerOrbit != null)
        {
            StartCoroutine(nameof(UpdateGemStatus), playerOrbit);
        }
        else
        {
            StartCoroutine(nameof(UpdateGemStatus));
        }
    }

    private IEnumerator UpdateGemStatus(PlayerOrbit playerOrbit)
    {
        foreach (var gem in GemHandler.GetInstance().allGems)
        {
            if(!gem.isCollected || gem.isReturned) continue;
            
            yield return new WaitForSeconds(2f);
            Debug.Log("Gem " + gem.gem.gemIdentifier + " - isC: " + gem.isCollected);
            gem.isReturned = true;

            Debug.Log("orb: " +playerOrbit);
            if (playerOrbit != null)
            {
                playerOrbit.RemoveGemFronOrbit(gem.gem);
            }
            
            foreach (GameObject spawnedGem in spawnedGems)
            {
                Gem gemComponent = spawnedGem.GetComponent<Gem>();

                if (gemComponent.gemIdentifier == gem.gem.gemIdentifier)
                {
                    Renderer rendererComponent = spawnedGem.GetComponent<Renderer>();
                    rendererComponent.material = gemComponent.normalMaterial;
                    
                    break;
                }
            }
        }
        
        GemHandler instance = GemHandler.GetInstance();
        Debug.Log(">>> : " + instance.allGems.Length + " - " + instance.numberOfCollectedGems);
        if (instance.allGems.Length == instance.numberOfCollectedGems)
        {
            Debug.Log("Ganhou!");
            yield return null;
        }
    }
    
    public class TowerGemInfo
    {
        public Gem Gem;
        public GameObject Prefab;
        public Boolean IsReturned;
    }
}
