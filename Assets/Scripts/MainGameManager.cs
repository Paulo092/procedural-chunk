using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainGameManager : MonoBehaviour
{
    private static MainGameManager _instance;
    public static MainGameManager GetInstance() => _instance;

    public Boolean hasWon;
    public GameObject winPanel;
    public GameObject pausePanel;

    public UIFade fadeUIElement;

    [Space] 
    public string collectedText = "Gemas coletadas";
    public string returnedText = "Gemas retornadas";
    public TextMeshProUGUI collectedTextMeshPro;
    public TextMeshProUGUI returnedTextMeshPro;

    private void Awake()
    {
        if (fadeUIElement != null)
        {
            fadeUIElement.gameObject.SetActive(true);
            fadeUIElement.TriggerFadeOut(3f);
        }
        
        if (_instance == null)
            _instance = this;
        
        if (winPanel == null)
            Debug.LogWarning("[MainGameHandler] Property 'winPanel' has been called but is null. Please assign it in the Inspector.");
        else
            winPanel.SetActive(false);
        
        GemHandler gemHandler = GemHandler.GetInstance();
        
        SetCollectedGemsText(0, gemHandler.allGems.Length);
        SetReturnedGemsText(0, gemHandler.allGems.Length);
    }

    private void OnDestroy()
    {
        ContinueGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if(pausePanel.activeSelf)
                ContinueGame();
            else
                PauseGame();
            
            SetUIInteractable(!pausePanel.activeSelf);
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    public void SetCollectedGemsText(int collectedAmount, int totalAmount)
    {
        collectedTextMeshPro.text = $"{collectedText} ({collectedAmount}/{totalAmount})";
    }
    
    public void SetReturnedGemsText(int returnedAmount, int totalAmount)
    {
        returnedTextMeshPro.text = $"{returnedText} ({returnedAmount}/{totalAmount})";
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
        pausePanel.SetActive(false);
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
