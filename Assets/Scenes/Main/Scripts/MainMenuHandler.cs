using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            FindObjectOfType<AudioManager>().StopAll();
            SceneManager.LoadScene("Menu");
        }
    }
}
