using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;
    public float jumpForce = 8.0f;
    public float gravityScale = 3.0f;
    public float rollDuration = 0.5f;
    public float rollSpeed = 6.0f; // Velocidade do rolamento
    public float rotationSpeed = 10f; // Velocidade de rota��o
    public Animator animator;
    public Transform cameraTransform;

    private Rigidbody rb;
    private UnityEngine.Vector3 moveDirection = UnityEngine.Vector3.zero;
    public bool isGrounded;
    public bool isRolling;
    private bool canRotate = true; // Controle de rota��o

    [Header("Ground & Air Detection Stats")]
    [SerializeField] float groundDetectionRayStartPoint = 0.5f;
    [SerializeField] float groundDetectionRayDistance = 0.6f; // Dist�ncia do Raycast para detectar o ch�o
    [SerializeField] float additionalRaycastDistance = 0.2f; // Dist�ncia adicional para inclina��es
    [SerializeField] LayerMask groundLayer; // Camada usada para verificar se est� no ch�o

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
            Debug.LogError("Animator n�o atribu�do no PlayerController.");
        }
        rb.freezeRotation = true; // Evita que o Rigidbody gire automaticamente

        // Verifica se o personagem est� no ar ao iniciar
        isGrounded = IsGrounded();
        if (!isGrounded)
        {
            // Configura��o inicial para o personagem no ar
            PlayTargetAnimation("Falling", true); // Ativar anima��o de queda
        }
    }

    void Update()
    {
        // Atualizar a verifica��o de se est� no ch�o
        bool wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (animator == null) return;

        if (isGrounded)
        {
            if (!wasGrounded) // Se acabamos de aterrissar
            {
                PlayTargetAnimation("Land", true);
                animator.SetBool("IsFalling", false); // Desativar anima��o de queda
            }

            HandleMovement();
            HandleActions();
        }
        else
        {
            moveDirection = UnityEngine.Vector3.zero;
            if (!isRolling && !animator.GetBool("IsFalling")) // S� ativa a anima��o se ainda n�o estiver rolando
            {
                PlayTargetAnimation("Falling", true); // Ativar anima��o de queda
                animator.SetBool("IsFalling", true);
            }
        }

        if (!isGrounded)
        {
            rb.velocity += UnityEngine.Vector3.down * gravityScale * Time.deltaTime;
        }

        UpdateAnimations();

        // Se estava no ch�o e agora n�o est� mais, inicie a anima��o de queda
        if (wasGrounded && !isGrounded)
        {
            PlayTargetAnimation("Falling", true);
            animator.SetBool("IsFalling", true);
        }
    }

    private void HandleMovement()
    {
        if (!canRotate) return; // Se a rota��o est� desativada, n�o processar movimento

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        UnityEngine.Vector3 forward = cameraTransform.forward;
        UnityEngine.Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")).normalized;
        moveDirection *= speed;

        // Aplicar o movimento horizontal e manter a velocidade vertical atual
        UnityEngine.Vector3 targetVelocity = new UnityEngine.Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        rb.velocity = UnityEngine.Vector3.Lerp(rb.velocity, targetVelocity, 0.1f); // Suavizar a mudan�a na velocidade

        // Girar o personagem para olhar na dire��o do movimento
        if (moveDirection != UnityEngine.Vector3.zero && canRotate)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleActions()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(UnityEngine.Vector3.up * jumpForce, ForceMode.Impulse);
            PlayTargetAnimation("Jump", true);
            isGrounded = false; // Impedir m�ltiplos pulos
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isRolling) // Evitar iniciar rolamento se j� estiver rolando
            {
                StartCoroutine(Roll());
            }
        }
    }

    private IEnumerator Roll() // Certifique-se de que o tipo IEnumerator � reconhecido
    {
        isRolling = true;
        animator.applyRootMotion = true; // Usar root motion para rolar
        PlayTargetAnimation("Rolling", true); // Acionar a anima��o de rolar
        float startTime = Time.time;
        UnityEngine.Vector3 initialDirection = moveDirection;
        while (Time.time < startTime + rollDuration)
        {
            rb.velocity = UnityEngine.Vector3.zero; // Parar o movimento f�sico durante o rolamento
            transform.position += initialDirection * Time.deltaTime * rollSpeed;
            yield return null;
        }
        animator.applyRootMotion = false; // Desativar root motion ap�s o rolamento
        isRolling = false;
    }

    private void UpdateAnimations()
    {
        bool isMoving = moveDirection.magnitude > 0;

        animator.SetFloat("Speed", isMoving ? (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) : 0);
        animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));

        // Definir IsJumping apenas se estiver no ar e subindo
        animator.SetBool("IsJumping", !isGrounded && rb.velocity.y > 0);
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        animator.applyRootMotion = isInteracting;
        animator.SetBool("IsInteracting", isInteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }

    private bool IsGrounded()
    {
        // Defina a posi��o do raycast no meio do personagem
        UnityEngine.Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        // Verifique o raycast principal
        if (Physics.Raycast(origin, UnityEngine.Vector3.down, out RaycastHit mainHit, groundDetectionRayDistance, groundLayer))
        {
            // Visualizar o raycast principal no editor para depura��o
            Debug.DrawRay(origin, UnityEngine.Vector3.down * groundDetectionRayDistance, Color.green);
            return true;
        }

        // Verifique com raycasts adicionais ao redor do personagem
        UnityEngine.Vector3[] raycastOffsets = {
        new UnityEngine.Vector3(0.5f, 0, 0),
        new UnityEngine.Vector3(-0.5f, 0, 0),
        new UnityEngine.Vector3(0, 0, 0.5f),
        new UnityEngine.Vector3(0, 0, -0.5f)
    };

        foreach (UnityEngine.Vector3 offset in raycastOffsets)
        {
            UnityEngine.Vector3 rayOrigin = origin + offset;
            if (Physics.Raycast(rayOrigin, UnityEngine.Vector3.down, out RaycastHit offsetHit, groundDetectionRayDistance, groundLayer))
            {
                // Visualizar os raycasts adicionais no editor para depura��o
                Debug.DrawRay(rayOrigin, UnityEngine.Vector3.down * groundDetectionRayDistance, Color.blue);
                return true;
            }
        }

        return false;
    }


    private void OnDrawGizmos()
    {
        UnityEngine.Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        // Gizmos para raycast principal
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, UnityEngine.Vector3.down * groundDetectionRayDistance);

        // Gizmos para raycasts adicionais
        UnityEngine.Vector3[] raycastOffsets = {
        new UnityEngine.Vector3(0.5f, 0, 0),
        new UnityEngine.Vector3(-0.5f, 0, 0),
        new UnityEngine.Vector3(0, 0, 0.5f),
        new UnityEngine.Vector3(0, 0, -0.5f)
    };

        foreach (UnityEngine.Vector3 offset in raycastOffsets)
        {
            UnityEngine.Vector3 rayOrigin = origin + offset;
            Gizmos.DrawRay(rayOrigin, UnityEngine.Vector3.down * groundDetectionRayDistance);
        }
    }
}
