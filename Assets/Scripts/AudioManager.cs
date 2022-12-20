using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    //UI Sounds
    Click,
    // Gameplay Sounds
    GameStart,
    DiskFall,
    Win,
    // Music
    BG_Music,
}

public class AudioManager : MonoBehaviour
{
    public enum AudioSourceType
    {
        UI,
        Gameplay,
        Music,
    }
    public static AudioManager Instance { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    

    [Serializable]
    public struct SoundRef
    {
        public SoundType type;
        public AudioClip clipRef;
    }

    [Serializable]
    public struct SoundSourceRef
    {
        public AudioSourceType SourceType;
        public AudioSource AudioSourceRef;
    }


    [SerializeField]
    private List<SoundRef> SoundRefList = new List<SoundRef>();

    [SerializeField]
    private List<SoundSourceRef> SoundSourceRefList = new List<SoundSourceRef>();


    public virtual void PlaySound(SoundType soundType, bool isLoop = false)
    {
        AudioClip clip = GetAudioClipByType(soundType);

        AudioSource source = GetAudioSourceBySoundType(soundType);

        source.loop = isLoop;

        if (isLoop && source.clip == clip)
        {
            return;
        }

        PlaySound(clip, source);

    }

    private void PlaySound(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }


    private AudioSource GetAudioSourceBySoundType(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Click:
                return GetAudioSourceByType(AudioSourceType.UI);
            case SoundType.Win:
            case SoundType.DiskFall:
            case SoundType.GameStart:
                return GetAudioSourceByType(AudioSourceType.Gameplay);
            case SoundType.BG_Music:
                return GetAudioSourceByType(AudioSourceType.Music);
            default:
                return GetAudioSourceByType(AudioSourceType.Gameplay);
        }
    }

    private AudioSource GetAudioSourceByType(AudioSourceType audioSourceType)
    {
        return SoundSourceRefList[(int)audioSourceType].AudioSourceRef;
    }

    private AudioClip GetAudioClipByType(SoundType soundType)
    {
        return SoundRefList[(int)soundType].clipRef;
    }

    private void OnValidate()
    {
        //todo - make the list automatically
        for (int i = 0; i < SoundRefList.Count; i++)
        {
            if ((SoundType)i != SoundRefList[i].type)
            {
                throw new Exception("sound references needs to be same order as the enum order");
            }
        }
        for (int i = 0; i < SoundSourceRefList.Count; i++)
        {
            if ((AudioSourceType)i != SoundSourceRefList[i].SourceType)
            {
                throw new Exception("sound source references needs to be same order as the enum order");
            }
        }
    }
}

