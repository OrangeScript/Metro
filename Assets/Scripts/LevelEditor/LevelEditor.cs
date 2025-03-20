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
        GetWindow<LevelEditor>("关卡编辑器");
    }

    private GameObject metroCarriagePrefab; // 允许用户拖拽预制体

    private void OnGUI()
    {
        GUILayout.Label("关卡编辑器", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        metroCarriagePrefab = (GameObject)EditorGUILayout.ObjectField("地铁车厢预制体", metroCarriagePrefab, typeof(GameObject), false);

        if (GUILayout.Button("添加地铁车厢"))
        {
            if (metroCarriagePrefab != null)
            {
                InstantiatePrefab(metroCarriagePrefab);
            }
            else
            {
                Debug.LogError("请先指定地铁车厢的预制体！");
            }
        }
    }

    void InstantiatePrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("预制体为空，无法实例化！");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (instance != null)
        {
            Undo.RegisterCreatedObjectUndo(instance, "创建地铁车厢"); // 支持撤销操作
            Selection.activeGameObject = instance; // 选中实例化的对象
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); // 标记场景为已修改
            Debug.Log("成功实例化地铁车厢！");
        }
        else
        {
            Debug.LogError("无法实例化预制体，请检查是否正确！");
        }
    }
}
