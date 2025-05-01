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
        EditorGUILayout.LabelField("=== ȼ�������� ===", EditorStyles.boldLabel);

        string status = item.isBurning ? "��ǰ״̬���ѵ�ȼ" : "��ǰ״̬��δȼ��";
        EditorGUILayout.HelpBox(status, MessageType.Info);

        if (item.visualSprite != null)
        {
            EditorGUILayout.ObjectField("��ǰ��ͼԤ��", item.visualSprite, typeof(Sprite), allowSceneObjects: false);
        }

        EditorGUILayout.Space();

        // ��ȼ��ť
        if (GUILayout.Button("��ȼ����"))
        {
            item.isBurning = true;

            // ֻ�ڳ���ʵ���в���
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
                Debug.LogWarning("������ Project ����Ԥ�����ϲ�����ȼ��ֻ�ڳ����е�ʵ����ʹ�á�");
            }
        }

        // ����ť
        if (GUILayout.Button("�������"))
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
                Debug.LogWarning("������ Project ����Ԥ�����ϲ�������ֻ�ڳ����е�ʵ����ʹ�á�");
            }
        }
    }
}
