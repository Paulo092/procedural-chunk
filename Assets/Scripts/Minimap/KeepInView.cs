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
        Vector2 player2dPosition = new Vector2(
            player.transform.position.x, 
            player.transform.position.z
        );

        Vector2 screenSize = new Vector2(
            Screen.width,
            Screen.height
        );

        Rect minimapRect = minimapImage.rectTransform.rect;
        
        float smallerCameraDimension = camera.orthographicSize;
        float smallerMinimapDimension = minimapRect.width < minimapRect.height ? minimapRect.width : minimapRect.height;
        

        float globalRadius = smallerCameraDimension / 2f;
        float minimapRadius = smallerMinimapDimension / 2f;

        float factor = globalRadius / minimapRadius;

        Vector3 playerPosition = player.transform.position;
        Vector3 targetPosition = target.transform.position;

        Vector3 unitaryVector = (targetPosition - playerPosition).normalized;

        Vector3 requiredPosition = Vector3.Distance(playerPosition, targetReference.transform.position) > 180
            ? new Vector3(
                playerPosition.x + unitaryVector.x * 180,
                targetPosition.y,
                playerPosition.z + unitaryVector.z * 180
            )
            : targetPosition;
        
        target.transform.position = requiredPosition;

        // // Distância do objeto à câmera
        // float distanceToCamera = Vector3.Distance(camera.transform.position, target.position);
        //
        // // Tamanho do campo de visão horizontal e vertical
        // float frustumHeight;
        // float frustumWidth;
        //
        // if (mainCamera.orthographic)
        // {
        //     // Cálculo para câmera ortográfica
        //     frustumHeight = mainCamera.orthographicSize * 2;
        //     frustumWidth = frustumHeight * mainCamera.aspect;
        // }
        // else
        // {
        //     // Cálculo para câmera perspectiva
        //     frustumHeight = 2.0f * distanceToCamera * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        //     frustumWidth = frustumHeight * mainCamera.aspect;
        // }
        //
        // // Posição desejada para manter o objeto dentro do campo de visão
        // Vector3 newPosition = target.position;
        //
        // // Limita a posição x do objeto
        // if (Mathf.Abs(newPosition.x - mainCamera.transform.position.x) > frustumWidth / 2)
        // {
        //     newPosition.x = mainCamera.transform.position.x + Mathf.Sign(newPosition.x - mainCamera.transform.position.x) * frustumWidth / 2;
        // }
        //
        // // Limita a posição y do objeto
        // if (Mathf.Abs(newPosition.y - mainCamera.transform.position.y) > frustumHeight / 2)
        // {
        //     newPosition.y = mainCamera.transform.position.y + Mathf.Sign(newPosition.y - mainCamera.transform.position.y) * frustumHeight / 2;
        // }
        //
        // // Aplica a nova posição ao objeto
        // target.position = newPosition;
    }
}
