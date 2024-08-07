using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGemCollect : MonoBehaviour
{
    public GameObject orbitCenter;
    public List<GameObject> spawnedGems = new();
    public Gem[] allGemsPrefabs;

    private void Start()
    {
        StartCoroutine(nameof(SpawnPlaceholderGem));
    }

    IEnumerator SpawnPlaceholderGem()
    {
        foreach (GemHandler.GemInfo gemInfo in GemHandler.GetInstance().allGems)
        {
            GameObject orbitalGemGameObject =
                Instantiate(gemInfo.gem.prefab, this.transform.position, Quaternion.identity);
            orbitalGemGameObject.SetActive(false);
            orbitalGemGameObject.transform.localScale = new Vector3(3f, 3f, 3f);

            GemOrbit gemOrbit = orbitalGemGameObject.AddComponent<GemOrbit>();
            gemOrbit.target = orbitCenter.transform;
            gemOrbit.orbitDistance = 20;

            Gem orbitalGem = orbitalGemGameObject.AddComponent<Gem>();
            gemInfo.gem.CloneInto(orbitalGem);
            orbitalGem.isNatural = false;

            Renderer orbitalGemRender = orbitalGemGameObject.GetComponent<Renderer>();
            orbitalGemRender.material = gemInfo.gem.notFoundedMaterial;

            orbitalGemGameObject.SetActive(true);

            spawnedGems.Add(orbitalGemGameObject);

            yield return new WaitForSeconds((360f / 30f) / GemHandler.GetInstance().allGems.Length);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerOrbit playerOrbit = other.GetComponent<PlayerOrbit>();

        if (playerOrbit != null)
        {
            StartCoroutine(nameof(UpdateGemStatus), playerOrbit);
        }
        else
        {
            StartCoroutine(nameof(UpdateGemStatus));
        }
    }

    private IEnumerator UpdateGemStatus(PlayerOrbit playerOrbit)
    {
        foreach (var gem in GemHandler.GetInstance().allGems)
        {
            if (!gem.isCollected || gem.isReturned) continue;

            yield return new WaitForSeconds(2f);
            // FindObjectOfType<AudioManager>().Play("SFX Return");
            if (GemHandler.GetInstance().returnSound != null)
                AudioSource.PlayClipAtPoint(GemHandler.GetInstance().returnSound, transform.TransformPoint(new Vector3(0, 0, 0)));

            
            gem.isReturned = true;

            if (playerOrbit != null)
            {
                playerOrbit.RemoveGemFronOrbit(gem.gem);
            }

            foreach (GameObject spawnedGem in spawnedGems)
            {
                Gem gemComponent = spawnedGem.GetComponent<Gem>();

                if (gemComponent.gemIdentifier == gem.gem.gemIdentifier)
                {
                    Renderer rendererComponent = spawnedGem.GetComponent<Renderer>();
                    rendererComponent.material = gemComponent.normalMaterial;

                    break;
                }
            }

            GemHandler gemHandlerInstance = GemHandler.GetInstance();

            MainGameManager.GetInstance().SetCollectedGemsText(
                gemHandlerInstance.GetCollectedGemsAmount() - gemHandlerInstance.GetReturnedGemsAmount(),
                gemHandlerInstance.GetGemsTotalAmount() - gemHandlerInstance.GetReturnedGemsAmount());

            MainGameManager.GetInstance().SetReturnedGemsText(
                gemHandlerInstance.GetReturnedGemsAmount(),
                gemHandlerInstance.GetGemsTotalAmount()
            );
        }

        GemHandler instance = GemHandler.GetInstance();
        if (instance.allGems.Length == instance.numberOfCollectedGems)
        {
            MainGameManager.GetInstance().ShowWinScreen();
            // Debug.Log("Ganhou!");
            yield return null;
        }
    }

    public class TowerGemInfo
    {
        public Gem Gem;
        public GameObject Prefab;
        public Boolean IsReturned;
    }
}