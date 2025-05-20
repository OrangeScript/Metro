using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgm;             // 背景音乐
    public AudioSource sfxSource;       // 一次性音效（如拾取）
    public AudioSource loopSfxSource;   // 循环音效通道（走路、匍匐、火焰共用）

    [Header("Audio Clips")]
    public AudioClip walkClip;
    public AudioClip crawlClip;
    public AudioClip fireClip;
    public AudioClip pickupItemClip;

    public bool IsBGMOn { get; private set; } = true;
    public float Volume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        sfxSource.volume = volume;
        loopSfxSource.volume = volume;
    }

    public void PlayPickupItemSFX()
    {
        if (pickupItemClip != null)
            sfxSource.PlayOneShot(pickupItemClip);
    }

    public void PlayLoopSFX(AudioClip clip)
    {
        if (clip == null) return;

        if (loopSfxSource.clip == clip && loopSfxSource.isPlaying)
            return;

        loopSfxSource.clip = clip;
        loopSfxSource.loop = true;
        loopSfxSource.Play();
    }
    public void StopLoopSFX()
    {
        loopSfxSource.Stop();
        loopSfxSource.clip = null;
    }
}
