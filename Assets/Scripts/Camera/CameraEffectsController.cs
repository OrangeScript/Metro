using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectsController : MonoBehaviour
{
    public Volume illusionVolume;
    public bool isIllusion;
    private PlayerController player;

    private void Start()
    {
        player= FindObjectOfType<PlayerController>();
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
        if (!(player.equippedItem is SmokeDetector smokeDetector)) { 
            UIManager.Instance.ShowMessage("������˻þ�����...");
        }
        else
        {
            UIManager.Instance.ShowMessage("Σ������������������");
        }
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

