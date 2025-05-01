using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CombustibleItem))]
public class CombustibleItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 

        CombustibleItem item = (CombustibleItem)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== 燃烧物设置 ===", EditorStyles.boldLabel);

        string status = item.isBurning ? "当前状态：已点燃" : "当前状态：未燃烧";
        EditorGUILayout.HelpBox(status, MessageType.Info);

        if (item.visualSprite != null)
        {
            EditorGUILayout.ObjectField("当前贴图预览", item.visualSprite, typeof(Sprite), allowSceneObjects: false);
        }

        EditorGUILayout.Space();

        // 点燃按钮
        if (GUILayout.Button("点燃物体"))
        {
            item.isBurning = true;

            // 只在场景实例中操作
            if (!PrefabUtility.IsPartOfPrefabAsset(item))
            {
                if (Application.isPlaying)
                {
                    item.Ignite();
                }
                else
                {
                    if (item.Flame != null)
                    {
                        item.Flame.transform.localPosition = item.flameOffset;
                        item.Flame.SetActive(true);
                    }
                    if (item.itemCollider != null)
                    {
                        item.itemCollider.isTrigger = false;  
                    }
                }
                EditorUtility.SetDirty(item);
            }
            else
            {
                Debug.LogWarning("请勿在 Project 面板的预制体上操作点燃，只在场景中的实例上使用。");
            }
        }

        // 扑灭按钮
        if (GUILayout.Button("扑灭火焰"))
        {
            item.isBurning = false;

            if (!PrefabUtility.IsPartOfPrefabAsset(item))
            {
                if (Application.isPlaying)
                {
                    item.Extinguish();
                }
                else
                {
                    if (item.Flame != null)
                    {
                        item.Flame.SetActive(false);
                    }

                    if (item.itemCollider != null)
                    {
                        item.itemCollider.isTrigger = true;  
                    }
                }
                EditorUtility.SetDirty(item);
            }
            else
            {
                Debug.LogWarning("请勿在 Project 面板的预制体上操作扑灭，只在场景中的实例上使用。");
            }
        }
    }
}
