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

        if (door.currentFault == MetroDoor.FaultType.Type3 ||
            door.currentFault == MetroDoor.FaultType.Type4 ||
            door.currentFault == MetroDoor.FaultType.Type5)
        {
            door.requiresBattery = EditorGUILayout.Toggle("��Ҫ���õ��", door.requiresBattery);
        }

        EditorGUILayout.Space();
        door.openSpeed = EditorGUILayout.FloatField("�����ٶ�", door.openSpeed);
        door.openSound = (AudioClip)EditorGUILayout.ObjectField("������Ч", door.openSound, typeof(AudioClip), false);
        door.jammedSound = (AudioClip)EditorGUILayout.ObjectField("��ס��Ч", door.jammedSound, typeof(AudioClip), false);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(door);
        }
    }
}
