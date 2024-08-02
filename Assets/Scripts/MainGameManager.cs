using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    private static MainGameManager _instance;
    public static MainGameManager GetInstance() => _instance;

    public Boolean hasWon;
    public GameObject winPanel;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        if (winPanel == null)
            Debug.LogWarning("[MainGameHandler] Property 'winPanel' has been called but is null. Please assign it in the Inspector.");
        else
            winPanel.SetActive(false);
    }
    
    public void ShowWinScreen()
    {
        if (winPanel == null)
            Debug.LogWarning(
                "MainGameHandler] Property 'winPanel' has been called but is null. Please assign it in the Inspector."
            );
        
        PauseGame();
        SetUIInteractable(true);
        winPanel.SetActive(true);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }
    
    private void ContinueGame()
    {
        Time.timeScale = 1;
    }

    private void SetUIInteractable(Boolean state)
    {
        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ContinuePlaying()
    {
        ContinueGame();
        SetUIInteractable(false);
        winPanel.SetActive(false);
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
