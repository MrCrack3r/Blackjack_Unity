using UnityEngine;

[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.7f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public SoundClip[] playerCardSounds;
    public SoundClip[] dealerCardSounds;
    public SoundClip[] flipSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayPlayerCardSound()
    {
        PlayRandom(playerCardSounds);
    }

    public void PlayDealerCardSound()
    {
        PlayRandom(dealerCardSounds);
    }

    public void PlayFlipSound()
    {
        PlayRandom(flipSounds);
    }

    private void PlayRandom(SoundClip[] sounds)
    {
        if (audioSource == null) return;
        if (sounds == null || sounds.Length == 0) return;

        int index = Random.Range(0, sounds.Length);
        SoundClip s = sounds[index];
        if (s == null || s.clip == null) return;

        audioSource.PlayOneShot(s.clip, s.volume);
    }
}