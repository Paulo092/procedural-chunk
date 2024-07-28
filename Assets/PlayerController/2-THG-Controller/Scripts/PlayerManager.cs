using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [Header("Animator Settings")]
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public Transform specialPoint;
    public int specialDamage = 50;
    public float cooldownTime = 5.0f;
    public float dodgeSpeed = 5.0f;
    public float dodgeDuration = 0.5f;
    public Transform target;
    public float targetSwitchRadius = 10f;
    public GameObject hitEffect;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public float moveSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float rotationSpeed = 720.0f;

    [Header("Combo Attack Settings")]
    public int comboStep = 0;
    public float comboResetTime = 1.0f;
    private float lastComboTime;
    public Queue<int> attackQueue = new Queue<int>();

    private Rigidbody rb;
    private bool isDodging = false;
    private bool isBlocking = false;
    private bool isSprinting = false;
    private float nextSpecialTime = 0f;
    private Transform[] targets;
    private int currentIndex = 0;
    private Vector3 moveDirection;

    [Header("Collider Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float fallSpeedLimit = 10f;
    [SerializeField] private float minFallDistance = 3f;

    private bool isGrounded;
    private bool wasGroundedLastFrame;

    public Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckGroundStatus();
        HandleMovement();
        HandleAnimation();
        HandleAttack();
        HandleSpecialAbility();
        HandleDodge();
        HandleAutoTarget();
        HandleSwitchTarget();

        // Update the IsGrounded parameter in the Animator
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        // Apply gravity only when not grounded
        if (!isGrounded)
        {
            rb.velocity += Physics.gravity * Time.deltaTime; // Apply gravity

            // Limit fall speed
            if (rb.velocity.y < -fallSpeedLimit)
            {
                rb.velocity = new Vector3(rb.velocity.x, -fallSpeedLimit, rb.velocity.z);
            }
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Zero vertical velocity
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position + Vector3.down * 0.1f, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGroundedLastFrame)
        {
            OnLanding();
        }

        wasGroundedLastFrame = isGrounded;

        if (rb.velocity.y < -minFallDistance)
        {
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }
    }

    private void HandleMovement()
    {
        if (isBlocking || isDodging)
        {
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(moveX, 0, moveZ).normalized;

        if (moveInput.magnitude >= 0.1f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            moveDirection = forward * moveInput.z + right * moveInput.x;

            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            isSprinting = Input.GetKey(KeyCode.LeftShift);
            float speed = isSprinting ? runSpeed : moveSpeed;
            Vector3 desiredVelocity = moveDirection * speed;
            rb.velocity = new Vector3(desiredVelocity.x, rb.velocity.y, desiredVelocity.z);

            animator.SetFloat("MoveDirectionX", moveInput.x);
            animator.SetFloat("MoveDirectionY", moveInput.z);
            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetBool("IsMoving", true);
            animator.SetBool("IsSprinting", isSprinting);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void HandleAnimation()
    {
        if (isDodging || isBlocking || animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsTag("SpecialAttack"))
        {
            animator.SetLayerWeight(0, 0);
        }
        else
        {
            animator.SetLayerWeight(0, 1);
        }
    }

    private void HandleAttack()
    {
        if (isBlocking || isDodging)
        {
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsTag("Attack");

        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            if (Time.time - lastComboTime <= comboResetTime)
            {
                comboStep++;
            }
            else
            {
                comboStep = 1;
            }
            lastComboTime = Time.time;

            if (isGrounded)
            {
                attackQueue.Enqueue(comboStep);
                if (attackQueue.Count == 1)
                {
                    Attack(attackQueue.Dequeue());
                }
            }
        }
    }

    private void HandleSpecialAbility()
    {
        if (Time.time >= nextSpecialTime && !isBlocking)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (isGrounded)
                {
                    UseSpecialAbility();
                    nextSpecialTime = Time.time + cooldownTime;
                }
            }
        }
    }

    private void HandleDodge()
    {
        if (Input.GetButtonDown("Dodge") && !isDodging && !isBlocking && animator.GetBool("IsMoving"))
        {
            StartCoroutine(Dodge());
        }
    }

    private void HandleAutoTarget()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) > targetSwitchRadius)
        {
            target = null;
        }

        if (target == null)
        {
            FindTarget();
        }
    }

    private void HandleSwitchTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (targets.Length > 0)
            {
                currentIndex = (currentIndex + 1) % targets.Length;
                target = targets[currentIndex];
            }
        }
    }

    private void Attack(int comboStep)
    {
        comboStep = Mathf.Clamp(comboStep, 1, 3);
        animator.SetTrigger("Attack" + comboStep);
        StartCoroutine(ProcessAttackQueue());

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
            }

            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }

    private IEnumerator ProcessAttackQueue()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (attackQueue.Count > 0)
        {
            Attack(attackQueue.Dequeue());
        }
    }

    private void UseSpecialAbility()
    {
        animator.SetTrigger("SpecialAttack");

        Collider[] hitEnemies = Physics.OverlapSphere(specialPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(specialDamage);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
            }

            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }

    private IEnumerator Dodge()
    {
        isDodging = true;
        animator.SetTrigger("Dodge");

        Vector3 dodgeDirection = moveDirection.normalized;
        rb.velocity = dodgeDirection * dodgeSpeed;

        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetSwitchRadius, enemyLayers);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = collider.transform;
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget;
        }
    }

    private void OnLanding()
    {
        animator.SetTrigger("IsLanding");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (specialPoint == null)
            return;

        Gizmos.DrawWireSphere(specialPoint.position, attackRange);
    }
}
