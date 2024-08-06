using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MinimapBehavior : MonoBehaviour
{
    public GameObject target;
    public GameObject rotationTarget;

    void Update()
    {
        Vector3 fixedPosition = new Vector3(
            target.transform.position.x, 
            this.transform.position.y,
            target.transform.position.z
        );

        Quaternion fixedRotation = new Quaternion(
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z,
            transform.rotation.w
        );

        fixedRotation = Quaternion.Euler(fixedRotation.eulerAngles.x, rotationTarget.transform.eulerAngles.y, fixedRotation.eulerAngles.z);
        
        this.transform.position = fixedPosition;
        this.transform.rotation = fixedRotation;
    }
}
