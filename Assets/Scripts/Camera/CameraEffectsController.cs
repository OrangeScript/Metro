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
        UIManager.Instance.ShowMessage("你进入了幻觉世界...");
    }

    public void ReturnFromIllusionWorld()
    {
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(false);
        }
        isIllusion = false;
        UIManager.Instance.ShowMessage("你恢复了意识。");
    }
}

