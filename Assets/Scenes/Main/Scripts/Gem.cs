
using System;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public string gemIdentifier;
    public GameObject prefab;
    
    public static List<Gem> CollectedGems = new();
    
    private void Start()
    {
        prefab = this.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        GemBag gemBag = other.GetComponent<GemBag>();

        if (gemBag != null)
        {
            gemBag.CollectGem(this);
            Destroy(this.gameObject);
        }
        
        CollectedGems.Add(this);
    }
}
