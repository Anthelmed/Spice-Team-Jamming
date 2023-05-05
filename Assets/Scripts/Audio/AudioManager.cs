using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    float tension;
    

    [Header("Sources")]
    [SerializeField] AudioSource[] playerSfxSources;
    [SerializeField] AudioSource[] mobSfxSources;
    [SerializeField] AudioSource uiAudioSource;
    [SerializeField] AudioSource overworldBgmSource;
    [SerializeField] AudioSource levelBgmSource;
    [SerializeField] AudioSource tensionSource;
    [SerializeField] Transform playerSfxSourceParent;
    [SerializeField] Transform mobSfxSourceParent;

    [Header("Clips")]
    [SerializeField] List<AudioClip> clips;
    [SerializeField] MultiSound[] multiSounds;



    [Header("Audio Mixer and Snapshots")]
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] AudioMixerSnapshot overWorldSnapshot;
    [SerializeField] AudioMixerSnapshot battleSnapshot;
    [SerializeField] AudioMixerSnapshot pauseSnapShot;
    [SerializeField] AudioMixerSnapshot blendedSnapshot;

    [Header("Music")]
    [SerializeField] bool shouldPlayBGM;
    [SerializeField] float transitionTime =3;

    int currentMobSource = 0;
    int currentPlayerSource = 0;

    public Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();
    public Dictionary<string, MultiSound> multiSoundDict = new Dictionary<string, MultiSound>();

    [Header("tension weights")]
    [SerializeField] float enemyWeight = 0.7f;
    [SerializeField] float playerWeight = 0.3f;

    GameState m_state;

  
    public void UpdateTension(float healthPercentage)
    {

        
        //float enemyPercentage = (float)numEnemiesOnScreen / totalPossibleEnemies;
        //float playerPercentage = (float)playerHealth / playerMaxHealth;
        //float combinedPercentage = (enemyPercentage * enemyWeight) + (playerPercentage * playerWeight);

        //tension = combinedPercentage;
      //  print("health eventsent to tension  " + percentage);
        float volume = Mathf.Lerp(-80f, 0f, 1- healthPercentage);
        bgmMixer.SetFloat("tension", volume);

    }

    void Awake()
    {
   
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
        
        }
        foreach (AudioClip clip in clips)
        {
            audioDict.Add(clip.name, clip);
        }
        foreach (MultiSound multi in multiSounds)
        {
            multiSoundDict.Add(multi.name, multi);
        }


        playerSfxSources = playerSfxSourceParent.GetComponentsInChildren<AudioSource>();
        mobSfxSources = mobSfxSourceParent.GetComponentsInChildren<AudioSource> ();

        if (shouldPlayBGM) StartBGM();

        PlayerStaticEvents.s_PlayerHealthChanged += UpdateTension;
    }


    private void Start()
    {
        if (GameManager.instance is null) return;
        GameManager.instance.OnGameStateChanged += HandleGameStateChange;

        bgmMixer.SetFloat("tension", 0);
   
    }
    private void StartBGM()
    {
       levelBgmSource.Play();
        overworldBgmSource.Play();
        tensionSource.Play();
    }
    GameState lastState;

    float mapCoolDown;
    float levelCoolDown;
    void HandleGameStateChange(GameState state)
    {
        m_state = state;
    


            switch (state)
            {
                case GameState.title:
                    {
                        InitMapMusic();
                        lastState = GameState.title;
                    }
                    break;
                case GameState.map:
                {
                    if (lastState == GameState.title) return;
                    else
                    {
                        if (Time.time > mapCoolDown)
                        {
                            mapCoolDown = Time.time + 20;
                            TransitionToMapMusic();
                        }

                    }
                }
                    break;
                case GameState.level:
                    {
                    if (Time.time > levelCoolDown)
                    {
                        levelCoolDown = Time.time + 10;
                        TransitionToLevelMusic();
                        lastState = GameState.level;
                    }
                    }
                    break;
                case GameState.pause:
                    {
                        TransitionToPauseMusic();
                        lastState = GameState.pause;
                    }
                    break;
                default:
                    break;
            }
        
    }
     void InitMapMusic()
    {
        overWorldSnapshot.TransitionTo(0);
    }

    public void TransitionToMapMusic()
    {
     //   StartCoroutine(CompleteFade(overWorldSnapshot));
        overWorldSnapshot.TransitionTo(1f);
    }

    public void TransitionToLevelMusic()
    {
        StartCoroutine(CompleteFade(battleSnapshot));
        blendedSnapshot.TransitionTo(transitionTime + 1);
       
    }

    public void TransitionToPauseMusic()
    {
        battleSnapshot.TransitionTo(transitionTime);
    }

    IEnumerator CompleteFade(AudioMixerSnapshot snapshot) // fake us an Scurve fade
    {
        yield return new WaitForSeconds(transitionTime);
        snapshot.TransitionTo(transitionTime);
    }


    public void PlaySingleClip(string clipName, SFXCategory category, float pitchVariance, float volumeVariance)
    {
        if (audioDict.ContainsKey(clipName)) RouteClipToSourcesAndPlay(audioDict[clipName], category, pitchVariance, volumeVariance);

        else Debug.Log("AudioManager: AudioClip " + clipName + " not found in dictionary.");
    }

    public void PlayShuffledSound(string soundName, SFXCategory category, float pitchVariance, float volumeVariance)
    {
        if (multiSoundDict.ContainsKey(soundName))
        {
            var tempClip = multiSoundDict[soundName].GetShuffledClip();
            if (tempClip != null) RouteClipToSourcesAndPlay(tempClip, category, pitchVariance, volumeVariance);
        }
        else
        {
            Debug.Log("AudioManager: AudioClip " + soundName + " not found in multi sound dictionary.");
        }
    }

    void RouteClipToSourcesAndPlay(AudioClip clip, SFXCategory category, float pitchVariance, float volumeVariance)
    {

        float pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        float volume = 1f + Random.Range(-volumeVariance, volumeVariance);

        switch (category)
        {
            case SFXCategory.player:
                {
            
                    if (m_state == GameState.map) return;
                    playerSfxSources[currentPlayerSource].pitch = pitch;
                    playerSfxSources[currentPlayerSource].volume = volume;
                    playerSfxSources[currentPlayerSource].PlayOneShot(clip);
                    currentPlayerSource = (currentPlayerSource + 1) % playerSfxSources.Length;
                }
                break;
            case SFXCategory.mob:
                {
                    if (m_state == GameState.map) return;
                    mobSfxSources[currentMobSource].pitch = pitch;
                    mobSfxSources[currentMobSource].volume = volume;
                    mobSfxSources[currentMobSource].PlayOneShot(clip);
                    currentMobSource = (currentMobSource + 1) % mobSfxSources.Length;
                }
                break;
            case SFXCategory.ui:
                {
                    uiAudioSource.pitch = pitch;
                    uiAudioSource.volume = volume;
                    uiAudioSource.PlayOneShot(clip);
                }

                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        PlayerStaticEvents.s_PlayerHealthChanged -= UpdateTension;
        GameManager.instance.OnGameStateChanged -= HandleGameStateChange;
    }

}

public enum SFXCategory
{
    player,
    mob,
    ui
}