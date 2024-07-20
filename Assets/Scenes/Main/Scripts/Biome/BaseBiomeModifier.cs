using UnityEngine;

[SerializeField]
public abstract class BaseBiomeModifier
{
    public abstract float GetHeight(Vector2 position);
}