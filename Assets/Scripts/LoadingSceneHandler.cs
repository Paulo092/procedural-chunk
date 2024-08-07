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
        // Verifica se o carregamento chegou a 0.9 e se qualquer tecla foi pressionada
        if (asyncLoad != null && asyncLoad.progress >= 0.9f && Input.anyKey)
        {            
            loadingTextTMP.text = waitText;
            asyncLoad.allowSceneActivation = true;
        }
    }

    IEnumerator LoadMainScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            // Calcula o progresso em uma escala de 0 a 100%
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f) * 100;
            loadingTextTMP.text = $"{loadingText} {progress:F0}%";

            // Verifica se o carregamento está completo (0.9) para exibir a mensagem de "pronto"
            if (asyncLoad.progress >= 0.9f)
            {
                loadingTextTMP.text = readyText;
            }

            yield return null;
        }
    }
}