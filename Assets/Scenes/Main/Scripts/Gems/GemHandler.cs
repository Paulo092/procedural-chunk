using System;
using UnityEngine;

public class GemHandler : MonoBehaviour
{
    public GemInfo[] allGems;
    public int numberOfCollectedGems = 0;

    private static GemHandler Instance;
    public static GemHandler GetInstance() => Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public static GemInfo GetGemInfo(Gem gem)
    {
        foreach (var currentGem in Instance.allGems)
        {
            if (currentGem.gem.gemIdentifier == gem.gemIdentifier) return currentGem;
        }

        return null;
    }

    public Boolean CollectGem(Gem gem)
    {
        GemInfo gemInfo = GetGemInfo(gem);

        if (gemInfo != null)
        {
            if(!gemInfo.isCollected)
                numberOfCollectedGems++;
            
            gemInfo.isCollected = true;
            return true;
        }

        return false;
    }

    [Serializable]
    public class GemInfo
    {
        public Gem gem;
        public Boolean isCollected;
        public Boolean isReturned;
    }
}
