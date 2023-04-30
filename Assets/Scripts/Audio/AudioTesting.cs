using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioTesting : MonoBehaviour
{
    public Dictionary<string, AudioClip> soundDictionary;
    public Transform buttonParent;  
    public GameObject soundButtonPrefab; 
    public string testMultiSound;
    public SFXCategory routingCategory;
    public float testPitchVariance;
    public float testVolVarince;

    void Start()
    {
        soundDictionary = AudioManager.instance.audioDict;
        foreach (KeyValuePair<string, AudioClip> clip in soundDictionary)
        {
            GameObject soundButton = Instantiate(soundButtonPrefab, buttonParent);
            soundButton.GetComponentInChildren<TMP_Text>().SetText(clip.Key);
            soundButton.GetComponent<Button>().onClick.AddListener(() => PlaySound(clip.Value));
        }
    }

    void PlaySound(AudioClip sound)
    {
        AudioManager.instance.PlaySingleClip(sound.name, SFXCategory.player, testPitchVariance,testVolVarince );
    }

  public  void TestMultiSound()
    {
        AudioManager.instance.PlayShuffledSound(testMultiSound, SFXCategory.player, testPitchVariance, testVolVarince);
    }
}
