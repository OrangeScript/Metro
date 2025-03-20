using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class LevelEditor : EditorWindow
{
    [MenuItem("Tools/Level Editor")]
    static void Init()
    {
        GetWindow<LevelEditor>("�ؿ��༭��");
    }

    private GameObject metroCarriagePrefab; // �����û���קԤ����

    private void OnGUI()
    {
        GUILayout.Label("�ؿ��༭��", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        metroCarriagePrefab = (GameObject)EditorGUILayout.ObjectField("��������Ԥ����", metroCarriagePrefab, typeof(GameObject), false);

        if (GUILayout.Button("��ӵ�������"))
        {
            if (metroCarriagePrefab != null)
            {
                InstantiatePrefab(metroCarriagePrefab);
            }
            else
            {
                Debug.LogError("����ָ�����������Ԥ���壡");
            }
        }
    }

    void InstantiatePrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Ԥ����Ϊ�գ��޷�ʵ������");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (instance != null)
        {
            Undo.RegisterCreatedObjectUndo(instance, "������������"); // ֧�ֳ�������
            Selection.activeGameObject = instance; // ѡ��ʵ�����Ķ���
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); // ��ǳ���Ϊ���޸�
            Debug.Log("�ɹ�ʵ�����������ᣡ");
        }
        else
        {
            Debug.LogError("�޷�ʵ����Ԥ���壬�����Ƿ���ȷ��");
        }
    }
}
