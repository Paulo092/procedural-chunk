using UnityEngine;

public class TeleportingPlataform : MonoBehaviour
{
    public GameObject teleportReference;

    private void OnTriggerEnter(Collider other)
    {
        if (teleportReference != null && other.GetComponent<CharacterController>() != null)
        {
            other.transform.position = teleportReference.transform.position;
        }
    }
}
