using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GemBags : MonoBehaviour
{
    public static List<Gem> collectedGems = new();

    // public static GemInfo[] AllGems;
    // public List<GameObject> collectedGemsPrefabs = new();

    public List<Gem> orbitGems = new();

    private void Start()
    {
        // foreach (GemHandler.GemInfo gemInfo in GemHandler.GetInstance().allGems)
        // {
        //     GameObject orbitalGemGameObject = Instantiate(gemInfo.gem.prefab, this.transform.position, Quaternion.identity);
        //     orbitalGemGameObject.SetActive(false);
        //     orbitalGemGameObject.transform.localScale = new Vector3(.1f, .1f, .1f);
        //
        //     GemOrbit gemOrbit = orbitalGemGameObject.AddComponent<GemOrbit>();
        //     gemOrbit.target = this.gameObject.transform;
        //     gemOrbit.orbitDistance = 2;
        //
        //     Gem orbitalGem = orbitalGemGameObject.AddComponent<Gem>();
        //     orbitalGem = gemInfo.gem;
        //     orbitalGem.isNatural = false;
        //     
        //     orbitGems.Add(orbitalGem);
        // }
    }

    public void CollectGem(Gem gem)
    {
        GameObject followGem = Instantiate(gem.prefab, this.transform.position, Quaternion.identity);
        followGem.transform.localScale = new Vector3(.1f, .1f, .1f);
        GemOrbit orbit = followGem.AddComponent<GemOrbit>();

        // gem.prefab = followGem;
        
        orbit.target = this.gameObject.transform;
        orbit.orbitDistance = 2;
        
        // collectedGemsPrefabs.Add(followGem);
        // collectedGems.Add(gem);

        Boolean ok = GemHandler.GetInstance().CollectGem(gem);
        Debug.Log(ok);
    }
}
