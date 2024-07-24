using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour
{
    public void StartGame(string SceneName)
    {
        SceneManager.LoadSceneAsync(SceneName);
    }
}
