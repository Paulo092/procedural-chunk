using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Quaternion = System.Numerics.Quaternion;

public class GemBag : MonoBehaviour
{
    public int NumberOfGems { get; private set; }
    public List<Gem> collectedGems = new();
    public UnityEvent<GemBag> OnGemCollected;

    public void CollectGem(Gem gem)
    {
        GameObject followGem = Instantiate(gem.prefab, this.transform.position, UnityEngine.Quaternion.identity);
        followGem.transform.localScale = new Vector3(.1f, .1f, .1f);
        PlayerGemOrbit orbit = followGem.AddComponent<PlayerGemOrbit>();

        gem.prefab = followGem;
        
        orbit.player = this.gameObject.transform;
        orbit.orbitDistance = 2;
        
        collectedGems.Add(gem);
    }
    
    public void GemCollected()
    {
        NumberOfGems++;
        OnGemCollected.Invoke(this);
    }
    
}
