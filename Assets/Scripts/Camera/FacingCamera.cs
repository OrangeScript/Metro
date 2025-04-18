using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    private List<Transform> childs;
    void Start()
    {
        childs =new List<Transform> ();
        for (int i = 0; i < transform.childCount; i++)
        {
            childs.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        //for (int i = 0; i < childs.Length; i++)
        //{
        //    childs[i].rotation = Camera.main.transform.rotation;
        //}
        childs.RemoveAll(child => child == null);
        Vector3 camEuler = Camera.main.transform.eulerAngles;
        foreach (Transform child in childs)
        {
            if (child == null) continue;
            Vector3 selfEuler = child.eulerAngles;
            child.rotation = Quaternion.Euler(camEuler.x, selfEuler.y, selfEuler.z);
        }
    }
}
