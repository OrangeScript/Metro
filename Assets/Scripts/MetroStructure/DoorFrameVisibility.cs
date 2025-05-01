using UnityEngine;

public class DoorFrameVisibility : MonoBehaviour
{
    public Camera mainCamera;           
    public float minDistance = 12f;      //�����ſ���ʧ����С����

    private Renderer doorFrameRenderer;

    void Start()
    {
        doorFrameRenderer = GetComponent<Renderer>();
        if (doorFrameRenderer == null)
        {
            Debug.LogError("Renderer not found on Door Frame.");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main; 
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        if (distance <= minDistance)
        {
            doorFrameRenderer.enabled = false;
        }
        else 
        {
            doorFrameRenderer.enabled = true;
        }
    }
}
