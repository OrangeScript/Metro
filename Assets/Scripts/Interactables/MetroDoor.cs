using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("门状态")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;

    [Header("门的属性")]
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
            Debug.Log("门被卡住了，无法打开！");
            if (jammedSound)
                audioSource.PlayOneShot(jammedSound);
            return;
        }

        Debug.Log("门正在打开...");
        currentState = DoorState.Open;
        animator.SetTrigger("Open");
        if (openSound)
            audioSource.PlayOneShot(openSound);
    }

    public void CloseDoor()
    {
        if (currentState == DoorState.Open)
        {
            Debug.Log("门正在关闭...");
            currentState = DoorState.Closed;
            animator.SetTrigger("Close");
        }
    }
}