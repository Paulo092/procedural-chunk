using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player; // Refer�ncia ao transform do jogador
    public float smoothSpeed = 0.125f; // Velocidade de suaviza��o da c�mera
    public float rotationSpeed = 5.0f; // Velocidade de rota��o da c�mera
    public float zoomSpeed = 4.0f; // Velocidade de zoom
    public float minZoom = 5.0f; // Zoom m�nimo
    public float maxZoom = 15.0f; // Zoom m�ximo
    public LayerMask collisionMask; // M�scara de colis�o para verificar obst�culos

    private float currentZoom = 10.0f; // Zoom atual
    private float mouseX, mouseY;
    private Vector3 desiredPosition;
    private const float verticalRotationMin = -80.0f; // Limite inferior de rota��o vertical
    private const float verticalRotationMax = 80.0f;  // Limite superior de rota��o vertical

    void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void LateUpdate()
    {
        // Atualizar a posi��o da c�mera com interpola��o suave
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Garantir que a c�mera esteja olhando para o jogador
        transform.LookAt(player);
    }

    void HandleRotation()
    {
        // Obter a entrada do mouse para rota��o
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Limitar a rota��o vertical
        mouseY = Mathf.Clamp(mouseY, verticalRotationMin, verticalRotationMax);

        // Calcular a rota��o da c�mera
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        transform.rotation = rotation;

        // Atualizar a posi��o desejada da c�mera com base no zoom atual
        UpdateCameraPosition();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Atualizar a posi��o da c�mera com base no zoom
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Atualizar a posi��o desejada da c�mera com base no zoom atual
        desiredPosition = player.position - transform.forward * currentZoom;

        // Verificar colis�es com o ambiente
        RaycastHit hit;
        if (Physics.Linecast(player.position, desiredPosition, out hit, collisionMask))
        {
            desiredPosition = hit.point;
        }
    }
}
