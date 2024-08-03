using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashSceneHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Referência ao VideoPlayer
    public string sceneName; // Nome da cena a ser carregada

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        
        // Adiciona o evento para ser chamado quando o vídeo terminar
        videoPlayer.loopPointReached += LoadScene;
    }

    void LoadScene(VideoPlayer vp)
    {
        // Carrega a nova cena
        SceneManager.LoadScene(sceneName);
    }
}
