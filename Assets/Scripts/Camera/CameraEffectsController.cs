using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectsController : MonoBehaviour
{
    public Volume illusionVolume;
    public bool isIllusion;


    private void Start()
    {
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(false);
        }
    }

    public void EnterIllusionWorld()
    {
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(true);
        }
        isIllusion = true;
        UIManager.Instance.ShowMessage("������˻þ�����...");
    }

    public void ReturnFromIllusionWorld()
    {
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(false);
        }
        isIllusion = false;
        UIManager.Instance.ShowMessage("��ָ�����ʶ��");
    }
}

