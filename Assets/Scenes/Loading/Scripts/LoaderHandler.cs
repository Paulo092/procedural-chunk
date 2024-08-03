using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoaderHandler : MonoBehaviour
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
        // Inicia o carregamento assíncrono da cena principal
        asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
        // asyncLoad.allowSceneActivation = false; // Não ativa a cena imediatamente

        // Enquanto a cena não estiver carregada
        while (!asyncLoad.isDone)
        {
            // Verifique o progresso se necessário
            // Debug.Log(asyncLoad.progress);

            // Você pode colocar aqui alguma lógica de atualização para o progresso
            yield return null;
        }

        // Adiciona um método para ativar a cena principal quando você quiser
        // Por exemplo, usando um botão ou outro evento para definir `allowSceneActivation` como true
    }

    public void OnLoadingComplete()
    {
        // Permite a ativação da cena
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
            loadingScreen.SetActive(false); // Desativa a tela de carregamento
        }
    }
}