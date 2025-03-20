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

        if (door.currentFault == MetroDoor.FaultType.Type3 ||
            door.currentFault == MetroDoor.FaultType.Type4 ||
            door.currentFault == MetroDoor.FaultType.Type5)
        {
            door.requiresBattery = EditorGUILayout.Toggle("需要备用电池", door.requiresBattery);
        }

        EditorGUILayout.Space();
        door.openSpeed = EditorGUILayout.FloatField("开门速度", door.openSpeed);
        door.openSound = (AudioClip)EditorGUILayout.ObjectField("开门音效", door.openSound, typeof(AudioClip), false);
        door.jammedSound = (AudioClip)EditorGUILayout.ObjectField("卡住音效", door.jammedSound, typeof(AudioClip), false);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(door);
        }
    }
}
