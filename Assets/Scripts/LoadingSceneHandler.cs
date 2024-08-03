using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneHandler : MonoBehaviour
{
    public string mainSceneName; // Nome da cena principal
    public GameObject loadingScreen; // Tela de carregamento

    private AsyncOperation asyncLoad;

    void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    IEnumerator LoadMainScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
        // asyncLoad.allowSceneActivation = false;

        // while (!asyncLoad.isDone)
        // {
        //     Debug.Log(asyncLoad.progress);
        //     yield return null;
        // }
        
        yield return null;
    }

    // public void OnLoadingComplete()
    // {
    //     if (asyncLoad != null)
    //     {
    //         asyncLoad.allowSceneActivation = true;
    //     }
    // }
}