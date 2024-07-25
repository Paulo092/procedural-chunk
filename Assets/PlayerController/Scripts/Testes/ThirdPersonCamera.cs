using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;         // Refer�ncia ao Transform do jogador
    public Transform cameraPivot;    // Refer�ncia ao pivot da c�mera
    public Transform mainCamera;     // Refer�ncia � c�mera principal
    public float distance = 5.0f;    // Dist�ncia da c�mera em rela��o ao jogador
    public float height = 2.0f;      // Altura da c�mera em rela��o ao jogador
    public float rotationSpeed = 5.0f; // Velocidade de rota��o da c�mera
    public float zoomSpeed = 2.0f;   // Velocidade do zoom
    public float minDistance = 2.0f; // Dist�ncia m�nima da c�mera
    public float maxDistance = 10.0f; // Dist�ncia m�xima da c�mera
    public float collisionBuffer = 0.1f; // Buffer de colis�o para evitar que a c�mera passe por objetos
    public LayerMask ignoreLayers;   // Camadas a serem ignoradas durante a colis�o

    private Vector3 currentRotation;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        // Inicializa a rota��o da c�mera
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

        // Atualizar a rota��o com base na entrada do mouse
        currentRotation.y += horizontalInput * rotationSpeed;
        currentRotation.x -= verticalInput * rotationSpeed;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -30f, 60f); // Limitar a rota��o vertical

        // Suavizar a rota��o
        Quaternion targetRotation = Quaternion.Euler(currentRotation);
        cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleCameraZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // Limitar a dist�ncia de zoom
    }

    private void UpdateCameraPosition()
    {
        // Calcular a posi��o desejada da c�mera com base na dist�ncia
        Vector3 direction = -mainCamera.forward * distance;
        targetPosition = player.position + direction + Vector3.up * height;
        mainCamera.position = Vector3.SmoothDamp(mainCamera.position, targetPosition, ref velocity, Time.deltaTime * 5f); // Interpola��o suave
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
