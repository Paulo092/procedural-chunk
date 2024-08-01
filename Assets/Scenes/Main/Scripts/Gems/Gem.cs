
using System;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public string gemIdentifier;
    public GameObject prefab;
    public Material normalMaterial;
    public Material notFoundedMaterial;
    
    // public static List<Gem> CollectedGems = new();
    public static List<Gem> AllGems = new();
    public Boolean isNatural = true;
    
    private void Start()
    {
        if (normalMaterial == null)
        {
            normalMaterial = this.GetComponent<Renderer>().material;
        }

        if (notFoundedMaterial == null)
        {
            notFoundedMaterial = new Material(normalMaterial);
            notFoundedMaterial.color = Color.white;
        }
        
        prefab = this.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isNatural) return;
        
        GemBag gemBag = other.GetComponent<GemBag>();

        if (gemBag != null)
        {
            gemBag.CollectGem(this);
            Destroy(this.gameObject);
        }
    }
}
