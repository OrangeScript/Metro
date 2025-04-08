using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;    // ֱ�ӹ���Ԥ����
        public int initialSize = 10;
    }

    [Header("���������")]
    [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();

    private Dictionary<GameObject, Queue<GameObject>> _prefabPoolMap = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �ӳٳ�ʼ��
            StartCoroutine(DelayedInitialization());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedInitialization()
    {
        yield return new WaitForEndOfFrame();
        InitializePools();
    }
    private void InitializePools()
    {
        foreach (var config in poolConfigs)
        {
            // ��ӿ����ü��
            if (config.prefab == null)
            {
                Debug.LogError($"��������ô��󣺷��ֿյ�Ԥ���������{poolConfigs.IndexOf(config)}��");
                continue;
            }

            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < config.initialSize; i++)
            {
                // ���ʵ��������
                try
                {
                    GameObject obj = Instantiate(config.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    pool.Enqueue(obj);
                    Debug.Log($"�ɹ���ʼ�� {config.prefab.name} ��");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"��ʼ��ʧ�ܣ�{config.prefab.name}\n{e.Message}");
                }
            }
            _prefabPoolMap.Add(config.prefab, pool);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("���Ի�ȡ��Ԥ����Ķ���");
            return null;
        }

        // ���Ԥ����δע�ᵽ������У���̬����һ��Ĭ�ϳ�
        if (!_prefabPoolMap.ContainsKey(prefab))
        {
            Debug.LogWarning($"δ�ҵ� {prefab.name} �Ķ���أ���̬����Ĭ�ϳ�");
            CreateNewPool(prefab, 1);
        }

        // ��������Ϊ�գ�����
        if (_prefabPoolMap[prefab].Count == 0)
        {
            Debug.Log($"���� {prefab.name} �����");
            ExpandPool(prefab);
        }

        GameObject obj = _prefabPoolMap[prefab].Dequeue();
        obj.transform.SetParent(null); // �������ظ���
        obj.SetActive(true); // ȷ������
        // ���ö����λ�ú���ת
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        return obj;
    }

    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if (prefab == null || obj == null)
        {
            Debug.LogError("���Է��ؿն��󵽶���أ�");
            return;
        }

        obj.SetActive(false); // ȷ�����󱻽���
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = true; // ��֤��Ⱦ������
        }
        obj.transform.SetParent(transform); // ���ص�����ع�������
        _prefabPoolMap[prefab].Enqueue(obj); // �����󷵻ص������
    }

    private void CreateNewPool(GameObject prefab, int initialSize)
    {
        if (prefab == null)
        {
            Debug.LogError("�����ÿ�Ԥ���崴�������");
            return;
        }

        // ������Ѵ���������
        if (_prefabPoolMap.ContainsKey(prefab))
        {
            Debug.LogWarning($"Ԥ���� {prefab.name} �Ķ�����Ѵ���");
            return;
        }

        // �����³�
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform); // ���ص���������
            pool.Enqueue(obj);
        }

        // ע�ᵽ�ֵ�
        _prefabPoolMap.Add(prefab, pool);
        Debug.Log($"�Ѷ�̬���� {prefab.name} ����أ���ʼ����={initialSize}");
    }


    private void ExpandPool(GameObject prefab)
    {
        // ÿ������3��ʵ��
        const int expandSize = 3;

        for (int i = 0; i < expandSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            _prefabPoolMap[prefab].Enqueue(obj);
        }

        Debug.Log($"{prefab.name} ����������ݣ���ǰ����={_prefabPoolMap[prefab].Count}");
    }
}