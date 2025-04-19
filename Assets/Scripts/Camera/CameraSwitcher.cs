using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class CameraSwitcher : MonoBehaviour
{
    // ����ģʽȷ��ȫ��Ψһ�������
    public static CameraSwitcher Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // ��ʼ��ʱ�Զ�������� AudioListener
        StartCoroutine(DelayedAudioListenerCheck());
    }

    private void Start()
    {
        // ��ʼ��ʱ�������� AudioListener
        EnforceSingleAudioListener(Camera.main);
    }

    // �ӳٵ���ȷ��������ʼ����ɺ��ټ�� AudioListener
    private IEnumerator DelayedAudioListenerCheck()
    {
        // �ȴ�һ֡��ȷ��������ʼ�����
        yield return null;
        EnforceSingleAudioListener(Camera.main);
    }

    public void SwitchToCamera(Camera newCamera)
    {
        if (newCamera == null)
        {
            Debug.LogError("�л�ʧ�ܣ�Ŀ�����������Ϊ null��");
            return;
        }

        // ������������������� Camera ���
        DisableAllCamerasExcept(newCamera);

        // ȷ��Ŀ�����������
        newCamera.enabled = true;

        // ǿ������ MainCamera ��ǩ
        UpdateMainCameraTag(newCamera);

        // ͬ�� AudioListener
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
        // ����� MainCamera ��ǩ
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam.CompareTag("MainCamera"))
            {
                cam.tag = "Untagged";
            }
        }

        // ������ MainCamera ��ǩ
        target.tag = "MainCamera";
    }

    private void EnforceSingleAudioListener(Camera targetCamera)
    {
        // ��ӡ���� AudioListener ��Ϣ����������
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();

        // �������� AudioListener������Ŀ��������� AudioListener
        foreach (AudioListener listener in listeners)
        {
            Debug.Log($"��ǰ AudioListener: {listener.gameObject.name}, ����״̬: {listener.enabled}");
            if (listener.gameObject != targetCamera.gameObject)
            {
                listener.enabled = false;
            }
        }

        // ȷ��Ŀ�����������ֻ��һ�� AudioListener
        AudioListener targetListener = targetCamera.GetComponent<AudioListener>();
        if (targetListener == null)
        {
            targetListener = targetCamera.gameObject.AddComponent<AudioListener>();
        }

        targetListener.enabled = true;

        // ��ֹ���л������г��ֶ�����õ� AudioListener
        foreach (AudioListener listener in listeners)
        {
            if (listener != targetListener && listener.enabled)
            {
                Debug.LogWarning($"��⵽����� AudioListener���ѽ��ã�{listener.name}");
                listener.enabled = false;
            }
        }
    }

    // ȷ����������ʱ������ֶ���� AudioListener
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
        // ȷ���³�������ʱ�������� AudioListener
        EnforceSingleAudioListener(Camera.main);
    }
}
