using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGemCollect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GemBag gemBag = other.GetComponent<GemBag>();

        if (gemBag != null)
        {
            if (gemBag.collectedGems.Count > 0)
            {
                foreach (var gem in gemBag.collectedGems)
                {
                    GameObject followGem = Instantiate(gem.prefab, this.transform.position, UnityEngine.Quaternion.identity);
                    followGem.transform.localScale = new Vector3(.1f, .1f, .1f);
                    PlayerGemOrbit orbit = followGem.AddComponent<PlayerGemOrbit>();
        
                    orbit.player = this.gameObject.transform;
                    orbit.orbitDistance = 2;
                }
            }
        }
    }
}
