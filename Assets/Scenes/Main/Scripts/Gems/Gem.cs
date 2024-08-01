
using System;
using System.Collections;
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

    private Boolean isTriggering = false;
    
    private void Awake()
    {
        if (gemIdentifier == null && prefab != null)
        {
            gemIdentifier = prefab.name;
        }
        
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
        // Debug.Log("Something Touch a Gem");
        
        if (other.CompareTag("Player") && !isTriggering)
        {
            // Debug.Log("Player Touch a Gem");

            // isTriggering = true;

            PlayerOrbit playerOrbit = other.GetComponent<PlayerOrbit>();
            playerOrbit.AddGemInOrbit(this);
            
            GemHandler.GetInstance().CollectGem(this);
            Destroy(this);
        }

        // StartCoroutine(nameof(ResetTriggering));
    }
    
    IEnumerator ResetTriggering()
    {
        yield return new WaitForEndOfFrame();
        isTriggering = false;
    }

    public void CloneInto(Gem otherGem)
    {
        otherGem.gemIdentifier = this.gemIdentifier;
        otherGem.prefab = this.prefab;
        otherGem.normalMaterial = this.normalMaterial;
        otherGem.notFoundedMaterial = this.notFoundedMaterial;
        otherGem.isNatural = this.isNatural;
    }
}
