using System;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public string gemIdentifier;
    public GameObject prefab;
    public Material normalMaterial;
    public Material notFoundedMaterial;
    public Boolean isNatural = true;

    private Boolean _isTriggering;
    
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
        
        if (other.CompareTag("Player") && !_isTriggering)
        {
            // FindObjectOfType<AudioManager>().Play("SFX Collect");
            PlayerOrbit playerOrbit = other.GetComponent<PlayerOrbit>();
            playerOrbit.AddGemInOrbit(this);
            
            GemHandler gemHandler = GemHandler.GetInstance();
            gemHandler.CollectGem(this);
            MainGameManager.GetInstance().SetCollectedGemsText(
                gemHandler.GetCollectedGemsAmount() - gemHandler.GetReturnedGemsAmount(), 
                gemHandler.GetGemsTotalAmount() - gemHandler.GetReturnedGemsAmount());

            Destroy(this);
        }
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
