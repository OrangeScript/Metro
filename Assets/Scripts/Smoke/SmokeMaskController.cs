using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeMaskController : MonoBehaviour
{
    public RectTransform maskRect;
    public Camera cam;
    public Transform player;
    public Material smokeMat;

    void Update()
    {
        Vector3 screenPos = cam.WorldToViewportPoint(player.position);
        smokeMat.SetVector("_PlayerPos", new Vector4(screenPos.x, screenPos.y, 0, 0));

        // ���ݵ�ǰλ�ü������ȼ����л� _MainTex
        SmokeSystem.SmokeLevel highestLevel = SmokeSystem.S.DetectHighestSmokeLevel(transform.position);
        //Texture2D maskTex = SmokeTextureLibrary.Instance.GetTextureForLevel(level);
        //smokeMat.SetTexture("_MainTex", maskTex);
    }
}

