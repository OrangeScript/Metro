using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("��״̬")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;

    [Header("�ŵ�����")]
    public bool requiresBattery = false;
    public float openSpeed = 1.0f;
    public AudioClip openSound;
    public AudioClip jammedSound;

    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        if (currentState == DoorState.Jammed)
        {
            Debug.Log("�ű���ס�ˣ��޷��򿪣�");
            if (jammedSound)
                audioSource.PlayOneShot(jammedSound);
            return;
        }

        Debug.Log("�����ڴ�...");
        currentState = DoorState.Open;
        animator.SetTrigger("Open");
        if (openSound)
            audioSource.PlayOneShot(openSound);
    }

    public void CloseDoor()
    {
        if (currentState == DoorState.Open)
        {
            Debug.Log("�����ڹر�...");
            currentState = DoorState.Closed;
            animator.SetTrigger("Close");
        }
    }
}