using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmokeSystem))]
public class SmokeSystemEditor : Editor
{
    private SerializedProperty activeSmokeProperty;

    void OnEnable()
    {
        activeSmokeProperty = serializedObject.FindProperty("activeSmoke");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.LabelField("Active Smoke Areas", EditorStyles.boldLabel);

        for (int i = 0; i < activeSmokeProperty.arraySize; i++)
        {
            SerializedProperty smokeArea = activeSmokeProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(smokeArea, new GUIContent($"Smoke Area {i + 1}"));

            if (GUILayout.Button($"Remove Smoke Area {i + 1}"))
            {
                activeSmokeProperty.DeleteArrayElementAtIndex(i);
                break;
            }
        }

        if (GUILayout.Button("Add New Smoke Area"))
        {
            activeSmokeProperty.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}