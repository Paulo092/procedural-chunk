using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoaderHandler : MonoBehaviour
{
    public string sceneName;
    public float initialWaitTime = 2f; // Tempo inicial antes de começar a carregar

    private void Start()
    {
        StartCoroutine(AsynchronousLoad(sceneName));
    }   
    
    IEnumerator AsynchronousLoad (string scene)
    {
        // Espera inicial para permitir a reprodução suave do vídeo
        yield return new WaitForSeconds(initialWaitTime);
        
        // Define a prioridade da operação de carregamento
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.priority = 2;
     
        while (!ao.isDone)
        {
            // Intervalo para reduzir a carga na CPU e permitir que o vídeo continue a ser reproduzido
            yield return new WaitForSeconds(0.1f);
        }
    }
}