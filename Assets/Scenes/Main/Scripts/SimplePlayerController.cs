using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;    // Velocidade de movimento do jogador
    public float mouseSensitivity = 2f;  // Sensibilidade do mouse para mover a câmera
    public Transform playerBody;    // Referência ao corpo do jogador para rotacionar

    float xAxisClamp = 0.0f;    // Clamp para limitar a rotação da câmera em X

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // Trava o cursor no centro da tela
    }

    void Update()
    {
        // Movimento do jogador
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Movimento da câmera (olhar ao redor)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xAxisClamp -= mouseY;
        float targetRotationX = playerBody.rotation.eulerAngles.x - mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            targetRotationX = 90.0f;
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            targetRotationX = 270.0f;
        }

        playerBody.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }
}
