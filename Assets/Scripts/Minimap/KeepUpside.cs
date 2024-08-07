using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUpside : MonoBehaviour
{
    public GameObject camera;
    
    void Update()
    {
        
        Quaternion fixedRotation = new Quaternion(
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z,
            transform.rotation.w
        );

        fixedRotation = Quaternion.Euler(
            fixedRotation.eulerAngles.x, 
            camera.transform.eulerAngles.y + 180, 
            fixedRotation.eulerAngles.z
        );
        
        this.transform.rotation = fixedRotation;
    }
}
