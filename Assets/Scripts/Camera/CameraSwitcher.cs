using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class CameraSwitcher : MonoBehaviour
{
    // 单例模式确保全局唯一操作入口
    public static CameraSwitcher Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // 初始化时自动清理多余 AudioListener
        StartCoroutine(DelayedAudioListenerCheck());
    }

    private void Start()
    {
        // 初始化时清理多余的 AudioListener
        EnforceSingleAudioListener(Camera.main);
    }

    // 延迟调用确保场景初始化完成后再检查 AudioListener
    private IEnumerator DelayedAudioListenerCheck()
    {
        // 等待一帧，确保场景初始化完成
        yield return null;
        EnforceSingleAudioListener(Camera.main);
    }

    public void SwitchToCamera(Camera newCamera)
    {
        if (newCamera == null)
        {
            Debug.LogError("切换失败：目标摄像机不能为 null！");
            return;
        }

        // 禁用所有其他摄像机的 Camera 组件
        DisableAllCamerasExcept(newCamera);

        // 确保目标摄像机启用
        newCamera.enabled = true;

        // 强制设置 MainCamera 标签
        UpdateMainCameraTag(newCamera);

        // 同步 AudioListener
        EnforceSingleAudioListener(newCamera);
    }

    private void DisableAllCamerasExcept(Camera exception)
    {
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam != exception)
            {
                cam.enabled = false;
            }
        }
    }

    private void UpdateMainCameraTag(Camera target)
    {
        // 清除旧 MainCamera 标签
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam.CompareTag("MainCamera"))
            {
                cam.tag = "Untagged";
            }
        }

        // 设置新 MainCamera 标签
        target.tag = "MainCamera";
    }

    private void EnforceSingleAudioListener(Camera targetCamera)
    {
        // 打印所有 AudioListener 信息，帮助调试
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();

        // 禁用所有 AudioListener，除了目标摄像机的 AudioListener
        foreach (AudioListener listener in listeners)
        {
            Debug.Log($"当前 AudioListener: {listener.gameObject.name}, 启用状态: {listener.enabled}");
            if (listener.gameObject != targetCamera.gameObject)
            {
                listener.enabled = false;
            }
        }

        // 确保目标摄像机有且只有一个 AudioListener
        AudioListener targetListener = targetCamera.GetComponent<AudioListener>();
        if (targetListener == null)
        {
            targetListener = targetCamera.gameObject.AddComponent<AudioListener>();
        }

        targetListener.enabled = true;

        // 防止在切换过程中出现多个启用的 AudioListener
        foreach (AudioListener listener in listeners)
        {
            if (listener != targetListener && listener.enabled)
            {
                Debug.LogWarning($"检测到多余的 AudioListener，已禁用：{listener.name}");
                listener.enabled = false;
            }
        }
    }

    // 确保场景加载时不会出现多余的 AudioListener
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 确保新场景加载时清理多余的 AudioListener
        EnforceSingleAudioListener(Camera.main);
    }
}
