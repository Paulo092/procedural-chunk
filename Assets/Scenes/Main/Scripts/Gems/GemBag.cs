using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GemBag : MonoBehaviour
{
    public static List<Gem> collectedGems = new();
    // public List<GameObject> collectedGemsPrefabs = new();

    public void CollectGem(Gem gem)
    {
        GameObject followGem = Instantiate(gem.prefab, this.transform.position, Quaternion.identity);
        followGem.transform.localScale = new Vector3(.1f, .1f, .1f);
        PlayerGemOrbit orbit = followGem.AddComponent<PlayerGemOrbit>();

        gem.prefab = followGem;
        
        orbit.player = this.gameObject.transform;
        orbit.orbitDistance = 2;
        
        // collectedGemsPrefabs.Add(followGem);
        collectedGems.Add(gem);
    }
}
