using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip forestAmbience;
    [SerializeField] private float ambienceVolume = 1f;

    private AudioSource soundSource;
    private AudioSource ambienceSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        soundSource = gameObject.AddComponent<AudioSource>();
        ambienceSource = gameObject.AddComponent<AudioSource>();

        ambienceSource.loop = true;
    }

    private void Start()
    {
        PlayAmbience(forestAmbience);
    }

    public void PlaySound(AudioClip clip)
    {
        PlaySound(clip, 1f);
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            soundSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null)
            return;

        ambienceSource.clip = clip;
        ambienceSource.volume = ambienceVolume;
        ambienceSource.Play();
    }
}