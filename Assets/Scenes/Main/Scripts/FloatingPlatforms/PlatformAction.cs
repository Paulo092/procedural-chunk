using System.Collections;
using UnityEngine;

public class PlatformAction : MonoBehaviour
{
    public enum ActionType
    {
        None,
        MoveHorizontal,
        MoveVertical,
        Rotate,
        ScalePingPong,
        FallAfterDelay
    }

    public ActionType actionType;
    public Vector3 movementDirection;
    public float movementSpeed;
    public AnimationCurve movementSpeedCurve;
    public Vector3 rotationAxis;
    public float rotationSpeed;
    public AnimationCurve rotationSpeedCurve;
    public Vector3 targetScale;
    public float scaleSpeed;
    public AnimationCurve scaleSpeedCurve;
    public float fallDelay;
    public float fallSpeed = 30f;
    public AnimationCurve fallSpeedCurve;
    public float pingPongDistance = 5f;

    public float horizontalStartDelay = 0f;
    public float verticalStartDelay = 0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isFalling;
    private float fallTimer;
    private bool isPlayerOnPlatform;
    private bool isHorizontalMovementStarted = false;
    private bool isVerticalMovementStarted = false;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        isFalling = false;
        fallTimer = 0f;
        isPlayerOnPlatform = false;

        if (actionType == ActionType.MoveHorizontal)
        {
            StartCoroutine(StartHorizontalMovementAfterDelay());
        }
        if (actionType == ActionType.MoveVertical)
        {
            StartCoroutine(StartVerticalMovementAfterDelay());
        }
    }

    private void Update()
    {
        switch (actionType)
        {
            case ActionType.MoveHorizontal:
                if (isHorizontalMovementStarted)
                {
                    MovePlatformPingPong();
                }
                break;
            case ActionType.MoveVertical:
                if (isVerticalMovementStarted)
                {
                    MovePlatformPingPong();
                }
                break;
            case ActionType.Rotate:
                RotatePlatform();
                break;
            case ActionType.ScalePingPong:
                ScalePlatformPingPong();
                break;
            case ActionType.FallAfterDelay:
                HandleFallingPlatform();
                break;
        }
    }

    private IEnumerator StartHorizontalMovementAfterDelay()
    {
        yield return new WaitForSeconds(horizontalStartDelay);
        isHorizontalMovementStarted = true;
    }

    private IEnumerator StartVerticalMovementAfterDelay()
    {
        yield return new WaitForSeconds(verticalStartDelay);
        isVerticalMovementStarted = true;
    }

    private float GetSpeed(float baseSpeed, AnimationCurve speedCurve)
    {
        if (speedCurve != null && speedCurve.keys.Length > 0)
        {
            return speedCurve.Evaluate(Time.time);
        }
        return baseSpeed;
    }

    private void MovePlatformPingPong()
    {
        float speed = GetSpeed(movementSpeed, movementSpeedCurve);
        float pingPong = Mathf.PingPong(Time.time * speed, pingPongDistance);
        transform.position = initialPosition + movementDirection.normalized * pingPong;
    }

    private void RotatePlatform()
    {
        float speed = GetSpeed(rotationSpeed, rotationSpeedCurve);
        transform.Rotate(rotationAxis * speed * Time.deltaTime);
    }

    private void ScalePlatformPingPong()
    {
        float speed = GetSpeed(scaleSpeed, scaleSpeedCurve);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, speed);
    }

    private void HandleFallingPlatform()
    {
        if (isFalling)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= fallDelay)
            {
                float speed = GetSpeed(fallSpeed, fallSpeedCurve);
                transform.position += Vector3.down * Time.deltaTime * speed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && actionType == ActionType.FallAfterDelay && isFalling == false)
        {
            isPlayerOnPlatform = true;
            isFalling = true;
            Debug.Log("Player detected. Platform will start falling.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && actionType == ActionType.FallAfterDelay)
        {
            isPlayerOnPlatform = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (actionType)
        {
            case ActionType.MoveHorizontal:
            case ActionType.MoveVertical:
                Gizmos.color = Color.red;
                Vector3 endPosition = initialPosition + movementDirection.normalized * pingPongDistance;
                Gizmos.DrawLine(initialPosition, endPosition);
                Gizmos.DrawSphere(initialPosition, 0.5f);
                Gizmos.DrawSphere(endPosition, 0.5f);
                break;
            case ActionType.Rotate:
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, rotationAxis * 2);
                break;
            case ActionType.ScalePingPong:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, targetScale);
                break;
            case ActionType.FallAfterDelay:
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(transform.position, Vector3.down * 2);
                break;
        }
    }
}
