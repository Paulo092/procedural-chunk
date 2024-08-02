using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlatformAction))]
public class PlatformActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlatformAction platformAction = (PlatformAction)target;

        platformAction.actionType = (PlatformAction.ActionType)EditorGUILayout.EnumPopup("Action Type", platformAction.actionType);

        EditorGUILayout.Space();

        switch (platformAction.actionType)
        {
            case PlatformAction.ActionType.MoveHorizontal:
            case PlatformAction.ActionType.MoveVertical:
                platformAction.movementDirection = EditorGUILayout.Vector3Field("Movement Direction", platformAction.movementDirection);
                platformAction.movementSpeed = EditorGUILayout.FloatField("Movement Speed", platformAction.movementSpeed);
                platformAction.movementSpeedCurve = EditorGUILayout.CurveField("Movement Speed Curve", platformAction.movementSpeedCurve);
                platformAction.pingPongDistance = EditorGUILayout.FloatField("Ping Pong Distance", platformAction.pingPongDistance);
                break;
            case PlatformAction.ActionType.Rotate:
                platformAction.rotationAxis = EditorGUILayout.Vector3Field("Rotation Axis", platformAction.rotationAxis);
                platformAction.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", platformAction.rotationSpeed);
                platformAction.rotationSpeedCurve = EditorGUILayout.CurveField("Rotation Speed Curve", platformAction.rotationSpeedCurve);
                break;
            case PlatformAction.ActionType.ScalePingPong:
                platformAction.targetScale = EditorGUILayout.Vector3Field("Target Scale", platformAction.targetScale);
                platformAction.scaleSpeed = EditorGUILayout.FloatField("Scale Speed", platformAction.scaleSpeed);
                platformAction.scaleSpeedCurve = EditorGUILayout.CurveField("Scale Speed Curve", platformAction.scaleSpeedCurve);
                break;
            case PlatformAction.ActionType.FallAfterDelay:
                platformAction.fallDelay = EditorGUILayout.FloatField("Fall Delay", platformAction.fallDelay);
                platformAction.fallSpeed = EditorGUILayout.FloatField("Fall Speed", platformAction.fallSpeed);
                platformAction.fallSpeedCurve = EditorGUILayout.CurveField("Fall Speed Curve", platformAction.fallSpeedCurve);
                break;
            default:
                break;
        }

        EditorUtility.SetDirty(platformAction);
    }
}
