#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioManager : MonoSingleton<AudioManager>
{
    List<AudioPlayer> players = new List<AudioPlayer>();
    const int CACHE_SOURCE_CONTROLL = 5;   

    public VolumeAdjustor volumeAdjustor;

    protected override void DoOnAwake()
    {
        base.DoOnAwake();
        if (players == null || players.Count == 0)
            GeneratePlayers(CACHE_SOURCE_CONTROLL);
        
        //DontDestroyOnLoad(this);
    }
    void GeneratePlayers(int amount)
    {
        // make sure all child are cleared
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        // generate new
        players.Clear();
        ExpandPlayerList(amount);
    }

    #region Public Play-Stop
    public void Play(AudioCommand command)
    {
        if (command == null)
        {
            Debug.LogWarning("Play null SourceAudio, no sound will be played !!!");
            return;
        }

        //Check max Instance
        if(command.limitCfg.maxInstancePertime > 0)
        {
            var currentPlayerForThisCommand = GetNumberOfPlayerArePlayingCommand(command);            
            if (currentPlayerForThisCommand >= command.limitCfg.maxInstancePertime)
            {
                Debug.Log("Audio reach limit, wont play this time");
                return;
            }
            
        }

        //Stop other Source
        if (command.stopOtherCfg.active)
        {
            for(int n= 0; n < players.Count; n++)
            {
                players[n].StopIfPlayAnyOfCommands(command.stopOtherCfg.others, command.stopOtherCfg.fadeOutDuration);
            }
        }

        // fadeOut BGM
        if (command.stopBgmCfg.active)
        {
            for (int n = 0, amount = players.Count; n < amount; n++)
            {
                players[n].StopIfPlayingType(AudioClipType.BGM, command.stopBgmCfg.fadeOutDuration);
            }
        }

        //
        // stop all
        if (command.stopAllCfg.active)
        {
            for (int n = 0, amount = players.Count; n < amount; n++)
            {
                players[n].Stop(command.stopAllCfg.fadeOutDuration);
            }
        }

        //------------ Play
        if (command.basicCfg.activePlayAudio)
        {
            var readyPlayer = GetReadyPlayer();
            readyPlayer.Play(command);
            //if (logAudioNameOnPlay)
            //{
            //    Log.Info(cmd, "Audio [{0}], time {1}, player[{2}]", cmd.name, Time.time, readyPlayer.name);
            //}
        }

        //----- Mute
        if (command.muteCfg.active)
        {
            //ActiveVolumeAll(!command.muteCfg.muteAll);
        }
    }
    #endregion

    #region Public - Mute
    public void ActiveVolumeAll(bool active)
    {
        volumeAdjustor.ActiveVolumeAll(active);
    }

    public void ActiveVolumeBGM(bool active)
    {
        volumeAdjustor.ActiveVolumeBGM(active);
    }

    public void ActiveVolumeSFX(bool active)
    {
        volumeAdjustor.ActiveVolumeSFX(active);
    }
    #endregion// Mute

    #region Public - Get Volume Value
    public float VolumeAllValue()
    {
        return volumeAdjustor.VolumeAllValue();
    }

    public float VolumeBGMValue()
    {
        return volumeAdjustor.VolumeBGMValue();
    }

    public float VolumeSFXValue()
    {
        return volumeAdjustor.VolumeSFXValue();
    }
    #endregion

    #region Public - Adjust
    public void AdjustVolumeAll(float volume)
    {
        volumeAdjustor.AdjustVolumeAll(volume);
    }

    public void AdjustVolumeBGM(float volume)
    {
        volumeAdjustor.AdjustVolumeBGM(volume);
    }

    public void AdjustVolumeSFX(float volume)
    {
        volumeAdjustor.AdjustVolumeSFX(volume);
    }
    #endregion

    #region Private
    AudioPlayer GetReadyPlayer()
    {
        // find the Idle source, if no source available, then expand sourceList
        var result = FindIdlePlayer();
        if (result == null)
        {
            ExpandPlayerList();
            // find again, sure we'll have one this time
            result = FindIdlePlayer();
        }
        return result;
    }
    AudioPlayer FindIdlePlayer()
    {
        return players.Find(x => !x.IsWorking());
    }

    void ExpandPlayerList(int amount = 1)
    {
        var currentPlayerCount = players.Count;
        for (int i = 0; i < amount; i++)
        {
            var playerObject = new GameObject("Player_" + (i + currentPlayerCount));
            playerObject.AddComponent<AudioSource>();
            
            players.Add(playerObject.AddComponent<AudioPlayer>());
            playerObject.transform.SetParent(transform);
        }
    }

    int GetNumberOfPlayerArePlayingCommand(AudioCommand command)
    {
        var sum = 0;
        for (int i = 0, amount = players.Count; i < amount; i++)
        {
            if (players[i].IsPlayingCommand(command))
                sum++;
        }
        return sum;
    }
    #endregion

    #region Class - AudioMixer
    [System.Serializable]
    public class VolumeAdjustor
    {
        #region Field, validate
        public AudioMixer mixer;
        /// Use this to adjust master volume of this mixer. NOTES : you must expose this parameter in Editor.
        /// Follow this : https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
        public string masterVolumeParameterName;
        public string bgmVolumeParameterName;
        public string sfxVolumeParameterName;
        [Header("Default value in Unity is [0, -80]")]
        [SerializeField] float maxVolumeDB = 0;
        [SerializeField] float minVolumeDB = -80;

        public void Validate()
        {
            //Log.Warning(mixer != null, "Missing AudioMixer");
            //Log.Warning(!string.IsNullOrEmpty(masterVolumeParameterName), "Missing masterVolumeParameterName");
            //Log.Warning(!string.IsNullOrEmpty(bgmVolumeParameterName), "Missing bgmVolumeParameterName");
            //Log.Warning(!string.IsNullOrEmpty(sfxVolumeParameterName), "Missing sfxVolumeParameterName");
        }

        public bool IsFaildedConfig()
        {
            return mixer == null
            || string.IsNullOrEmpty(masterVolumeParameterName)
            || string.IsNullOrEmpty(bgmVolumeParameterName)
            || string.IsNullOrEmpty(sfxVolumeParameterName);
        }
        #endregion//Field, validate


        #region Public - mute
        public void ActiveVolumeAll(bool active)
        {
            if (IsFaildedConfig())
                return;
            mixer.SetFloat(masterVolumeParameterName, active ? maxVolumeDB : minVolumeDB);
        }

        public void ActiveVolumeBGM(bool active)
        {
            if (IsFaildedConfig())
                return;
            mixer.SetFloat(bgmVolumeParameterName, active ? maxVolumeDB : minVolumeDB);
        }

        public void ActiveVolumeSFX(bool active)
        {
            if (IsFaildedConfig())
                return;
            mixer.SetFloat(sfxVolumeParameterName, active ? maxVolumeDB : minVolumeDB);
        }
        #endregion//Public - mute

        #region Public - GetInfo
        public bool IsMuteAll()
        {
            float masterVol;
            mixer.GetFloat(masterVolumeParameterName, out masterVol);
            return masterVol.Equals(0);
                
        }

        public bool IsMuteBGM()
        {
            float bgmVol;
            mixer.GetFloat(bgmVolumeParameterName, out bgmVol);
            return bgmVol.Equals(0);
        }

        public bool IsMuteSFX()
        {
            float sfxVol;
            mixer.GetFloat(sfxVolumeParameterName, out sfxVol);
            return sfxVol.Equals(0);
        }
        #endregion//Public - GetInfo

        #region Public GetVolume
        public float VolumeAllValue()
        {
            float masterVol;
            mixer.GetFloat(masterVolumeParameterName, out masterVol);
            return masterVol;

        }

        public float VolumeBGMValue()
        {
            float bgmVol;
            mixer.GetFloat(bgmVolumeParameterName, out bgmVol);
            return bgmVol;

        }

        public float VolumeSFXValue()
        {
            float sfxVol;
            mixer.GetFloat(bgmVolumeParameterName, out sfxVol);
            return sfxVol;

        }
        #endregion

        #region Public - Volume adjust
        public void AdjustVolumeAll(float volume)
        {
            if (IsFaildedConfig())
                return;
            mixer.SetFloat(masterVolumeParameterName, volume);
        }

        public void AdjustVolumeBGM(float volume)
        {
            if (IsFaildedConfig())
                return;
            mixer.SetFloat(bgmVolumeParameterName, volume);
        }

        public void AdjustVolumeSFX(float volume)
        {
            if (IsFaildedConfig())
                return;
            mixer.SetFloat(sfxVolumeParameterName, volume);
        }
        #endregion
    }
    #endregion
   
}

public enum AudioClipType
{
    SFX =0,
    BGM =1,
    VoiceLine =2,
}
