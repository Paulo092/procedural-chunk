using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureValidade : MonoBehaviour
{
    public Gem structureGem;
    public GemHandler.GemInfo globalGemEquivalent;
    
    private void Start()
    {
        foreach (var gemInfo in GemHandler.GetInstance().allGems)
        {
            if (gemInfo.gem.gemIdentifier == structureGem.gemIdentifier)
            {
                globalGemEquivalent = gemInfo;
            }
        }
    }

    void Update()
    {
        if(globalGemEquivalent.isCollected) Destroy(this.gameObject);
    }
}
