using System.Collections.Generic;
using UnityEngine;

public class PlayerOrbit : MonoBehaviour
{
    public List<Gem> gemsInOrbit = new();
    
    public List<Gem> orbitGems = new();

    private void Start()
    {
        foreach (GemHandler.GemInfo gemInfo in GemHandler.GetInstance().allGems)
        {
            GameObject orbitalGemGameObject = Instantiate(gemInfo.gem.prefab, this.transform.position, Quaternion.identity);
            orbitalGemGameObject.SetActive(false);
            orbitalGemGameObject.transform.localScale = new Vector3(.1f, .1f, .1f);
        
            GemOrbit gemOrbit = orbitalGemGameObject.AddComponent<GemOrbit>();
            gemOrbit.target = this.gameObject.transform;
            gemOrbit.orbitDistance = 2;

            Gem orbitalGem = orbitalGemGameObject.AddComponent<Gem>();
            gemInfo.gem.CloneInto(orbitalGem);
            orbitalGem.isNatural = false;
            
            orbitGems.Add(orbitalGem);
        }
    }

    public void AddGemInOrbit(Gem gem)
    {
        foreach (Gem gemInOrbit in orbitGems)
            if (gemInOrbit.gemIdentifier == gem.gemIdentifier)
            {
                gemInOrbit.gameObject.SetActive(true);
                return;
            }
    }
    
    public void RemoveGemFronOrbit(Gem gem)
    {
        foreach (Gem gemInOrbit in orbitGems)
            if (gemInOrbit.gemIdentifier == gem.gemIdentifier)
            {
                gemInOrbit.gameObject.SetActive(false);
                return;
            }
    }
    
}
