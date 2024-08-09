using UnityEngine;

public class TeleportingPlataform : MonoBehaviour
{
    public GameObject teleportReference;

    private void OnTriggerStay(Collider other)
    {
        if (teleportReference != null && other.GetComponent<CharacterController>() != null)
        {
            other.transform.position = teleportReference.transform.position;
        }
    }
}
