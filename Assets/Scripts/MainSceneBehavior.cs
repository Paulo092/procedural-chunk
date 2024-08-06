using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MainSceneBehavior : MonoBehaviour
{
    public string[] musicNames;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InvokeRepeating("ChooseMusic", 3f, 180f);
    }

    public void ChooseMusic()
    {
        Random rand = new((int) Time.realtimeSinceStartupAsDouble);
        
        // AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        // int max = audioManager.sounds.Length;
        // int musicIndex = rand.Next(0, max);
        //
        // audioManager.Play(audioManager.sounds[musicIndex].name);
        
        if(musicNames.Length > 0)
            FindObjectOfType<AudioManager>().Play(musicNames[rand.Next(0, musicNames.Length)]);
        
    }

}
