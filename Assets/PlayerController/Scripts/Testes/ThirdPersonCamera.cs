using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;         // Referência ao Transform do jogador
    public Transform cameraPivot;    // Referência ao pivot da câmera
    public Transform mainCamera;     // Referência à câmera principal
    public float distance = 5.0f;    // Distância da câmera em relação ao jogador
    public float height = 2.0f;      // Altura da câmera em relação ao jogador
    public float rotationSpeed = 5.0f; // Velocidade de rotação da câmera
    public float zoomSpeed = 2.0f;   // Velocidade do zoom
    public float minDistance = 2.0f; // Distância mínima da câmera
    public float maxDistance = 10.0f; // Distância máxima da câmera
    public float collisionBuffer = 0.1f; // Buffer de colisão para evitar que a câmera passe por objetos
    public LayerMask ignoreLayers;   // Camadas a serem ignoradas durante a colisão

    private Vector3 currentRotation;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        // Inicializa a rotação da câmera
        currentRotation = transform.eulerAngles;

        // Ocultar o cursor do mouse e trancar ele no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleCameraRotation();
        HandleCameraZoom();
        UpdateCameraPosition();
        HandleCameraCollision();
    }

    private void HandleCameraRotation()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // Atualizar a rotação com base na entrada do mouse
        currentRotation.y += horizontalInput * rotationSpeed;
        currentRotation.x -= verticalInput * rotationSpeed;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -30f, 60f); // Limitar a rotação vertical

        // Suavizar a rotação
        Quaternion targetRotation = Quaternion.Euler(currentRotation);
        cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleCameraZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // Limitar a distância de zoom
    }

    private void UpdateCameraPosition()
    {
        // Calcular a posição desejada da câmera com base na distância
        Vector3 direction = -mainCamera.forward * distance;
        targetPosition = player.position + direction + Vector3.up * height;
        mainCamera.position = Vector3.SmoothDamp(mainCamera.position, targetPosition, ref velocity, Time.deltaTime * 5f); // Interpolação suave
    }

    private void HandleCameraCollision()
    {
        RaycastHit hit;
        Vector3 direction = (mainCamera.position - cameraPivot.position).normalized;
        float checkDistance = distance;

        if (Physics.Raycast(player.position, direction, out hit, checkDistance, ~ignoreLayers))
        {
            float distanceToHit = Vector3.Distance(player.position, hit.point) - collisionBuffer;
            mainCamera.position = player.position + direction * Mathf.Clamp(distanceToHit, minDistance, checkDistance);
        }
    }
}
