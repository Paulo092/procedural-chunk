using System;
using UnityEngine;

public class GemHandler : MonoBehaviour
{
    public GemInfo[] allGems;
    public int numberOfCollectedGems;
    public AudioClip collectSound, returnSound;
    
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
            if (!gemInfo.isCollected)
            {
                if (collectSound != null)
                    AudioSource.PlayClipAtPoint(collectSound, gem.transform.TransformPoint(new Vector3(0, 0, 0)));
                    
                numberOfCollectedGems++;
            }
            
            gemInfo.isCollected = true;
            return true;
        }

        return false;
    }

    public int GetCollectedGemsAmount()
    {
        int counter = 0;
        foreach (GemInfo gem in allGems)
            if (gem.isCollected)
                counter++;

        return counter;
    }
    public int GetReturnedGemsAmount()
    {
        int counter = 0;
        foreach (GemInfo gem in allGems)
            if (gem.isReturned)
                counter++;

        return counter;
    }

    public int GetGemsTotalAmount()
    {
        return allGems.Length;
    }

    [Serializable]
    public class GemInfo
    {
        public Gem gem;
        public Boolean isCollected;
        public Boolean isReturned;
    }
}
