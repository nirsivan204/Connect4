using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region PublicEnums
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
#endregion
/// <summary>
/// This class manages all the music in game
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region PrivateEnums
    [Serializable]
    private enum AudioSourceType
    {
        UI,
        Gameplay,
        Music,
    }
    #endregion

    #region Structs

    [Serializable]
    struct SoundRef
    {
        public SoundType type;
        public AudioClip clipRef;
    }

    [Serializable]
    struct SoundSourceRef
    {
        public AudioSourceType SourceType;
        public AudioSource AudioSourceRef;
    }
    #endregion

    public static AudioManager Instance { get; private set; }

    #region ScriptLifeCycleFunctions

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            //load player prefs
            float musicVol, SFXVol;
            GameData.LoadPlayerMusicPrefs(out musicVol, out SFXVol);
            SetMusicVol(musicVol);
            SetSFXVol(SFXVol);
            DontDestroyOnLoad(this);
        }
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
    #endregion


    [SerializeField]
    private List<SoundRef> SoundRefList = new List<SoundRef>();

    [SerializeField]
    private List<SoundSourceRef> SoundSourceRefList = new List<SoundSourceRef>();

    #region PublicMethods
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

    public void SetMusicVol(float val)
    {
        SoundSourceRefList[(int)AudioSourceType.Music].AudioSourceRef.volume = val;
    }

    public void SetSFXVol(float val)
    {
        SoundSourceRefList[(int)AudioSourceType.UI].AudioSourceRef.volume = val;
        SoundSourceRefList[(int)AudioSourceType.Gameplay].AudioSourceRef.volume = val;
    }
    #endregion
    #region PrivateMethods
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
    #endregion



}

