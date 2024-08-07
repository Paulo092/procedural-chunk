using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingSceneHandler : MonoBehaviour
{
    public string mainSceneName; // Nome da cena principal
    public TextMeshProUGUI loadingTextTMP;
    public GameObject loadingScreen; // Tela de carregamento

    public string loadingText, readyText, waitText;
    
    private AsyncOperation asyncLoad;

    void Start()
    {
        StartCoroutine(LoadMainScene());
        loadingTextTMP.text = loadingText;
    }

    private void Update()
    {
        if (asyncLoad != null && asyncLoad.progress >= 0.9f && Input.anyKey)
        {            
            asyncLoad.allowSceneActivation = true;
        }
    }

    IEnumerator LoadMainScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f) * 100;
            loadingTextTMP.text = $"{loadingText} {progress:F0}%";

            if (asyncLoad.progress >= 0.9f)
            {
                loadingTextTMP.text = readyText;
            }

            yield return null;
        }
        
        yield return null;
    }
}