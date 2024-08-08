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

        if (globalGemEquivalent.isCollected)
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (globalGemEquivalent.isCollected)
        {
            MainGameManager.GetInstance().SpawnExplosion(this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
