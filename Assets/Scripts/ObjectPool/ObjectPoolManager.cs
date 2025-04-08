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
        public GameObject prefab;    // 直接关联预制体
        public int initialSize = 10;
    }

    [Header("对象池配置")]
    [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();

    private Dictionary<GameObject, Queue<GameObject>> _prefabPoolMap = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 延迟初始化
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
            // 添加空引用检查
            if (config.prefab == null)
            {
                Debug.LogError($"对象池配置错误：发现空的预制体项（索引{poolConfigs.IndexOf(config)}）");
                continue;
            }

            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < config.initialSize; i++)
            {
                // 添加实例化保护
                try
                {
                    GameObject obj = Instantiate(config.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    pool.Enqueue(obj);
                    Debug.Log($"成功初始化 {config.prefab.name} 池");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"初始化失败：{config.prefab.name}\n{e.Message}");
                }
            }
            _prefabPoolMap.Add(config.prefab, pool);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("尝试获取空预制体的对象！");
            return null;
        }

        // 如果预制体未注册到对象池中，动态创建一个默认池
        if (!_prefabPoolMap.ContainsKey(prefab))
        {
            Debug.LogWarning($"未找到 {prefab.name} 的对象池，动态创建默认池");
            CreateNewPool(prefab, 1);
        }

        // 如果对象池为空，扩容
        if (_prefabPoolMap[prefab].Count == 0)
        {
            Debug.Log($"扩容 {prefab.name} 对象池");
            ExpandPool(prefab);
        }

        GameObject obj = _prefabPoolMap[prefab].Dequeue();
        obj.transform.SetParent(null); // 解除对象池父级
        obj.SetActive(true); // 确保激活
        // 重置对象的位置和旋转
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        return obj;
    }

    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if (prefab == null || obj == null)
        {
            Debug.LogError("尝试返回空对象到对象池！");
            return;
        }

        obj.SetActive(false); // 确保对象被禁用
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = true; // 保证渲染器启用
        }
        obj.transform.SetParent(transform); // 挂载到对象池管理器下
        _prefabPoolMap[prefab].Enqueue(obj); // 将对象返回到对象池
    }

    private void CreateNewPool(GameObject prefab, int initialSize)
    {
        if (prefab == null)
        {
            Debug.LogError("尝试用空预制体创建对象池");
            return;
        }

        // 如果池已存在则跳过
        if (_prefabPoolMap.ContainsKey(prefab))
        {
            Debug.LogWarning($"预制体 {prefab.name} 的对象池已存在");
            return;
        }

        // 创建新池
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform); // 挂载到管理器下
            pool.Enqueue(obj);
        }

        // 注册到字典
        _prefabPoolMap.Add(prefab, pool);
        Debug.Log($"已动态创建 {prefab.name} 对象池，初始容量={initialSize}");
    }


    private void ExpandPool(GameObject prefab)
    {
        // 每次扩容3个实例
        const int expandSize = 3;

        for (int i = 0; i < expandSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            _prefabPoolMap[prefab].Enqueue(obj);
        }

        Debug.Log($"{prefab.name} 对象池已扩容，当前容量={_prefabPoolMap[prefab].Count}");
    }
}