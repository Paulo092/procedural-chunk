using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXDestroyer : MonoBehaviour
{
    private VisualEffect _visualEffect;

    void Start()
    {
        _visualEffect = GetComponent<VisualEffect>();
        
        if (_visualEffect == null)
            Debug.LogError("No VisualEffect component found on this GameObject.");
        else
            Destroy(gameObject, 5f);
    }
}
