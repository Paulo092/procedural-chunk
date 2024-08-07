using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeepInView : MonoBehaviour
{
    public Camera camera; // A câmera principal
    public Transform target; // O objeto a ser mantido dentro do campo de visão
    public GameObject targetReference;
    public GameObject player;
    public Image minimapImage;

    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 targetPosition = target.transform.position;

        Vector3 unitaryVector = (targetPosition - playerPosition).normalized;

        Vector3 requiredPosition = Vector3.Distance(playerPosition, targetReference.transform.position) > 180
            ? new Vector3(
                playerPosition.x + unitaryVector.x * 180,
                targetPosition.y,
                playerPosition.z + unitaryVector.z * 180
            )
            : targetReference.transform.position;
        
        target.transform.position = requiredPosition;
    }
}
