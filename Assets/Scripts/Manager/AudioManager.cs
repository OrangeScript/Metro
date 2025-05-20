using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgm;             // ��������
    public AudioSource sfxSource;       // һ������Ч����ʰȡ��
    public AudioSource loopSfxSource;   // ѭ����Чͨ������·�����롢���湲�ã�

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
