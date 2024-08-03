using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehavior : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<AudioManager>().Play("MainMenuMusic");
    }
}
