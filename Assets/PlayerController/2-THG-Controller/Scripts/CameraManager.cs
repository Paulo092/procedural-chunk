using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player; // Referência ao transform do jogador
    public float smoothSpeed = 0.125f; // Velocidade de suavização da câmera
    public float rotationSpeed = 5.0f; // Velocidade de rotação da câmera
    public float zoomSpeed = 4.0f; // Velocidade de zoom
    public float minZoom = 5.0f; // Zoom mínimo
    public float maxZoom = 15.0f; // Zoom máximo
    public LayerMask collisionMask; // Máscara de colisão para verificar obstáculos

    private float currentZoom = 10.0f; // Zoom atual
    private float mouseX, mouseY;
    private Vector3 desiredPosition;
    private const float verticalRotationMin = -80.0f; // Limite inferior de rotação vertical
    private const float verticalRotationMax = 80.0f;  // Limite superior de rotação vertical

    void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void LateUpdate()
    {
        // Atualizar a posição da câmera com interpolação suave
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Garantir que a câmera esteja olhando para o jogador
        transform.LookAt(player);
    }

    void HandleRotation()
    {
        // Obter a entrada do mouse para rotação
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Limitar a rotação vertical
        mouseY = Mathf.Clamp(mouseY, verticalRotationMin, verticalRotationMax);

        // Calcular a rotação da câmera
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        transform.rotation = rotation;

        // Atualizar a posição desejada da câmera com base no zoom atual
        UpdateCameraPosition();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Atualizar a posição da câmera com base no zoom
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Atualizar a posição desejada da câmera com base no zoom atual
        desiredPosition = player.position - transform.forward * currentZoom;

        // Verificar colisões com o ambiente
        RaycastHit hit;
        if (Physics.Linecast(player.position, desiredPosition, out hit, collisionMask))
        {
            desiredPosition = hit.point;
        }
    }
}
