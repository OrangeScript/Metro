using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource bgm;
    public bool IsBGMOn { get; private set; } = true;
    public float Volume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetBGMState(bool isOn)
    {
        IsBGMOn = isOn;
        bgm.mute = !isOn;
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
        bgm.volume = volume;
    }
}
