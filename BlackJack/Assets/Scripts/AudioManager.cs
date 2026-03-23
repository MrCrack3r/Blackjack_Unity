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

    [Header("Žaidėjo kortos garsai")]
    public SoundClip[] playerCardSounds;

    [Header("Dalintojo kortos garsai")]
    public SoundClip[] dealerCardSounds;

    [Header("Kortos apvertimo garsai")]
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
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayPlayerCardSound()
    {
        if (playerCardSounds == null || playerCardSounds.Length == 0) return;
        int index = Random.Range(0, playerCardSounds.Length);
        audioSource.PlayOneShot(playerCardSounds[index].clip, playerCardSounds[index].volume);
    }

    public void PlayDealerCardSound()
    {
        if (dealerCardSounds == null || dealerCardSounds.Length == 0) return;
        int index = Random.Range(0, dealerCardSounds.Length);
        audioSource.PlayOneShot(dealerCardSounds[index].clip, dealerCardSounds[index].volume);
    }

    public void PlayFlipSound()
    {
        if (flipSounds == null || flipSounds.Length == 0) return;
        int index = Random.Range(0, flipSounds.Length);
        audioSource.PlayOneShot(flipSounds[index].clip, flipSounds[index].volume);
    }
}