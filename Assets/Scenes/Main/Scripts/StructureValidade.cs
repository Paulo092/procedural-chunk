using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureValidade : MonoBehaviour
{
    public Gem structureGem;
    void Update()
    {
        foreach (var collectedGem in Gem.CollectedGems)
        {
            if (structureGem.gemIdentifier == collectedGem.gemIdentifier)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
