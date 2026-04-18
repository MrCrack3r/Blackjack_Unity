using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip introClip;
    public AudioClip loopClip;
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
        SoundClip targetMusic = null;

        // PATIKRINK AR ŠIE PAVADINIMAI SUTAMPA SU TAVO SCENŲ PAVADINIMAIS UNITYJE
        if (sceneName == "Main_menu_scene") targetMusic = mainMenuMusic;
        else if (sceneName == "Backjack_table_scene") targetMusic = gameplayMusic;
        else if (sceneName == "Shop") targetMusic = shopMusic; // Pridėta shop scena

        if (targetMusic != null)
        {
            // Apsauga nuo muzikos persikrovimo perjungiant langus
            if (musicSource.clip != null &&
               (musicSource.clip == targetMusic.introClip || musicSource.clip == targetMusic.loopClip) &&
               musicSource.isPlaying)
            {
                return;
            }

            SwitchMusic(targetMusic);
        }
    }

    private void SetupSources()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
    }

    // ŠIS METODAS SUTVARKO TAVO KLAIDĄ CS0103
    private void SwitchMusic(SoundClip s)
    {
        if (musicCoroutine != null) StopCoroutine(musicCoroutine);
        musicCoroutine = StartCoroutine(PlayMusicWithIntro(s));
    }

    public void PlayMenuMusic() { SwitchMusic(mainMenuMusic); }
    public void PlayGameplayMusic() { SwitchMusic(gameplayMusic); }
    public void PlayLoseMusic() { SwitchMusic(loseMusic); }
    public void PlayShopMusic() { SwitchMusic(shopMusic); }

    private IEnumerator PlayMusicWithIntro(SoundClip s)
    {
        if (s == null || (s.introClip == null && s.loopClip == null)) yield break;

        musicSource.Stop();
        musicSource.loop = false;
        musicSource.volume = s.volume * masterMusicVolume;

        if (s.introClip != null)
        {
            musicSource.clip = s.introClip;
            musicSource.Play();
            yield return new WaitWhile(() => musicSource.isPlaying);
        }

        if (s.loopClip != null)
        {
            musicSource.clip = s.loopClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // --- SFX ---
    public void PlayPlayerCardSound() { PlayRandom(playerCardSounds); }
    public void PlayDealerCardSound() { PlayRandom(dealerCardSounds); }
    public void PlayFlipSound() { PlayRandom(flipSounds); }

    private void PlayRandom(SoundClip[] sounds)
    {
        if (sounds == null || sounds.Length == 0) return;
        SoundClip s = sounds[Random.Range(0, sounds.Length)];
        AudioClip clipToPlay = s.loopClip != null ? s.loopClip : s.introClip;
        if (clipToPlay != null) sfxSource.PlayOneShot(clipToPlay, s.volume * masterSFXVolume);
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