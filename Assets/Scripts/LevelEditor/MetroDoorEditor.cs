using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MetroDoor))]
public class MetroDoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MetroDoor door = (MetroDoor)target;
        if (door == null) return;

        EditorGUILayout.LabelField("�ŵ�����", EditorStyles.boldLabel);
        door.currentState = (MetroDoor.DoorState)EditorGUILayout.EnumPopup("��ǰ��״̬", door.currentState);
        door.currentFault = (MetroDoor.FaultType)EditorGUILayout.EnumPopup("��ǰ��������", door.currentFault);

        EditorGUILayout.Space();
        door.openSpeed = EditorGUILayout.FloatField("�����ٶ�", door.openSpeed);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(door);
        }
    }
}
