using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip introClip; // Pradinė dalis (gali būti null)
    public AudioClip loopClip;  // Pagrindinė dalis, kuri kartosis
    [Range(0f, 1f)] public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Garsų grupės")]
    public SoundClip[] playerCardSounds;
    public SoundClip[] dealerCardSounds;
    public SoundClip[] flipSounds;

    [Header("Muzikos įrašai")]
    public SoundClip mainMenuMusic;
    public SoundClip gameplayMusic;
    public SoundClip loseMusic;
    public SoundClip shopMusic;

    [Header("Bendra kontrolė")]
    [Range(0f, 1f)] public float masterMusicVolume = 0.5f;
    [Range(0f, 1f)] public float masterSFXVolume = 0.7f;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private Coroutine musicCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckAndPlaySceneMusic();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlaySceneMusic();
    }

    private void CheckAndPlaySceneMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Main_menu_scene") PlayMenuMusic();
        else if (sceneName == "Backjack_table_scene") PlayGameplayMusic();
    }

    private void SetupSources()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMenuMusic() { StartCoroutine(PlayMusicWithIntro(mainMenuMusic)); }
    public void PlayGameplayMusic() { StartCoroutine(PlayMusicWithIntro(gameplayMusic)); }
    public void PlayLoseMusic() { StartCoroutine(PlayMusicWithIntro(loseMusic)); }
    public void PlayShopMusic() { StartCoroutine(PlayMusicWithIntro(shopMusic)); }

    private IEnumerator PlayMusicWithIntro(SoundClip s)
    {
        if (s == null || (s.introClip == null && s.loopClip == null)) yield break;

        // Jei ta pati muzika jau groja, nieko nedarom
        if (musicSource.clip != null && (musicSource.clip == s.introClip || musicSource.clip == s.loopClip) && musicSource.isPlaying)
            yield break;

        musicSource.Stop();
        musicSource.loop = false;
        musicSource.volume = s.volume * masterMusicVolume;

        // 1. Grojam Intro (jei jis yra)
        if (s.introClip != null)
        {
            musicSource.clip = s.introClip;
            musicSource.Play();

            // Laukiame, kol baigsis intro
            yield return new WaitWhile(() => musicSource.isPlaying);
        }

        // 2. Grojam Loop
        if (s.loopClip != null)
        {
            musicSource.clip = s.loopClip;
            musicSource.loop = true; // Čia įjungiam begalinį kartojimą
            musicSource.Play();
        }
    }

    public void PlayPlayerCardSound() { PlayRandom(playerCardSounds); }
    public void PlayDealerCardSound() { PlayRandom(dealerCardSounds); }
    public void PlayFlipSound() { PlayRandom(flipSounds); }

    private void PlayRandom(SoundClip[] sounds)
    {
        if (sounds == null || sounds.Length == 0) return;
        SoundClip s = sounds[Random.Range(0, sounds.Length)];
        if (s != null && s.loopClip != null) // Naudojame loopClip kaip pagrindinį SFX
        {
            sfxSource.PlayOneShot(s.loopClip, s.volume * masterSFXVolume);
        }
    }

    public void SetMasterMusicVolume(float volume)
    {
        masterMusicVolume = volume;
        if (musicSource != null) musicSource.volume = volume;
    }

    public void SetMasterSFXVolume(float volume) { masterSFXVolume = volume; }

    public void PauseMusic() { if (musicSource.isPlaying) musicSource.Pause(); }
    public void UnPauseMusic() { musicSource.UnPause(); }
}