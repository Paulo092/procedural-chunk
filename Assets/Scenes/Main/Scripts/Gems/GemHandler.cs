using System;
using UnityEngine;

public class GemHandler : MonoBehaviour
{
    public GemInfo[] allGems;
    public int numberOfCollectedGems;

    private static GemHandler _instance;
    public static GemHandler GetInstance() => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public static GemInfo GetGemInfo(Gem gem)
    {
        foreach (var currentGem in _instance.allGems)
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
