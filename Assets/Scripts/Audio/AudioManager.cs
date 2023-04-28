using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sources")]
    [SerializeField] AudioSource[] playerSfxSources;
    [SerializeField] AudioSource[] mobSfxSources;
    [SerializeField] AudioSource uiAudioSource;
    [SerializeField] AudioSource[] bgmSources;
    [SerializeField] Transform playerSfxSourceParent;
    [SerializeField] Transform mobSfxSourceParent;

    [Header("Clips")]
    [SerializeField] List<AudioClip> clips;
    [SerializeField] MultiSound[] multiSounds;


    [Header("Audio Mixer and Snapshots")]
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] AudioMixer mobChatterMixer;
    [SerializeField] AudioMixerSnapshot overWorldSnapshot;
    [SerializeField] AudioMixerSnapshot battleSnapshot;
    [SerializeField] AudioMixerSnapshot lightMobs;
    [SerializeField] AudioMixerSnapshot medMobs;
    [SerializeField] AudioMixerSnapshot heavyMobs;

    int currentMobSource = 0;
    int currentPlayerSource = 0;

    public Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();
    public Dictionary<string, MultiSound> multiSoundDict = new Dictionary<string, MultiSound>();



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

        StartBGM();
    }

    private void StartBGM()
    {
        //set up mixer snapshots and play both loops
    }

    public void PlayOverworldMusic()
    {
       
    }

    public void PlayBattleMusic()
    {
        // mixer snapshot fade
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
                    playerSfxSources[currentPlayerSource].pitch = pitch;
                    playerSfxSources[currentPlayerSource].volume = volume;
                    playerSfxSources[currentPlayerSource].PlayOneShot(clip);
                    currentPlayerSource = (currentPlayerSource + 1) % playerSfxSources.Length;
                }
                break;
            case SFXCategory.mob:
                {
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

}

public enum SFXCategory
{
    player,
    mob,
    ui
}