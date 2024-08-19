using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Sound Effects")]
    public AudioClip[] soundEffects;
    private AudioSource soundEffectSource;
    [Range(0f, 1f)] public float soundEffectVolume; // Volume for sound effects

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    private AudioSource musicSource;
    [Range(0f, 1f)] public float musicVolume; // Volume for background music

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Set up audio sources
        soundEffectSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        PlayBackgroundMusic();
    }

    public void PlaySoundEffect(string soundName)
    {
        AudioClip clip = GetClipByName(soundName, soundEffects);
        if (clip != null)
        {
            soundEffectSource.volume = soundEffectVolume;
            soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{soundName}' not found.");
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Background music clip not assigned.");
        }
    }

    private AudioClip GetClipByName(string name, AudioClip[] clips)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
}
