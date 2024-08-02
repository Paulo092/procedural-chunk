using UnityEngine;

public class GemOrbit : MonoBehaviour
{
    public Transform target;
    public float orbitDistance = 5f;
    public float orbitSpeed = 30f;
    public float followSpeed = 2f;
    public Vector3 heightOffset = new Vector3(0f, 1f, 0f);

    private Vector3 _offset;

    void Start()
    {
        _offset = new Vector3(orbitDistance, 0, 0);
    }

    private void Awake()
    {
        this.transform.position = target.position;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0);
        _offset = rotation * _offset;
        Vector3 desiredPosition = target.position + _offset + heightOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
