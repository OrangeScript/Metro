using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MetroDoor))]
public class MetroDoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MetroDoor door = (MetroDoor)target;
        if (door == null) return;

        EditorGUILayout.LabelField("门的设置", EditorStyles.boldLabel);
        door.currentState = (MetroDoor.DoorState)EditorGUILayout.EnumPopup("当前门状态", door.currentState);
        door.currentFault = (MetroDoor.FaultType)EditorGUILayout.EnumPopup("当前故障类型", door.currentFault);

        EditorGUILayout.Space();
        door.openSpeed = EditorGUILayout.FloatField("开门速度", door.openSpeed);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(door);
        }
    }
}
