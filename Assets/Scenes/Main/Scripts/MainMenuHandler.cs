using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            Debug.Log("Entrei");
            SceneManager.LoadScene("Menu");
        }
    }
}
