using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    #region Init
    AudioSource source;
    AudioCommand command;
    AudioClipType clipType;
    float volume;

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        ResetAudioSource();
    }
    #endregion


    #region Public Play - Stop
    public void Play(AudioCommand command) 
    {
        if (command == null) return;

        //reset audio source first
        ResetAudioSource();
        this.command = command;
        this.clipType = command.basicCfg.clipType;
        this.source.clip = command.GetClipForPlay();
        source.outputAudioMixerGroup = command.basicCfg.mixerGroup;
        volume = 1;
              
        StartPlayingProcess(command);

    }

    public void Stop(float fadeOutDuration = 0)
    {
        if (!IsWorking()) return;
        if (fadeOutDuration > 0)
        {
            StartFadeOutProcess(fadeOutDuration);
        }
        else
        {
            source.Stop();            
            ResetAudioSource();
        }
    }

    public void StopIfPlayAnyOfCommands(AudioCommand[] commands, float fadeOutDuration)
    {
        if (!IsWorking() || commands == null || (commands != null && commands.Length == 0)) return;
        for (int n = 0; n < commands.Length; n++)
        {
            if (command == commands[n])
            {
                Stop(fadeOutDuration);
                break;
            }
        }
    }

    public void StopIfPlayingType(AudioClipType clipType, float fadeOutDuration)
    {
        if (!IsWorking()) return;
        if (this.clipType == clipType) Stop(fadeOutDuration);
    }
    #endregion

    #region Public Checking
    public bool IsWorking()
    {
        return source.isPlaying || isFadeProcess || isPlayProcess;
    }

    public bool IsPlayingCommand(AudioCommand command)
    {
        if (command == null) return false;
        else return (IsWorking() && this.command == command);
    }

    #endregion

    #region Private Play-Stop
    bool isPlayProcess = false;
    bool isFadeProcess = false;

    void StartPlayingProcess(AudioCommand command)
    {
        StartCoroutine(IE_PlayingProcess(command));
    }
    IEnumerator IE_PlayingProcess(AudioCommand command)
    {
        isPlayProcess = true;
        //Delay
        if (command.startCfg.delay > 0)
        {
            float _duration = command.startCfg.delay;
            float _timer = 0.0f;

            while (_timer <= _duration)
            {                
                _timer += Time.deltaTime;
                yield return null;
            }
        }
        //Play
        source.Play();
        //FadeInEffect
        if(command.startCfg.fadeInDuration > 0)
        {
            float _initial = 0;
            float _target = volume;
            source.volume = 0f;
            float _duration = command.startCfg.fadeInDuration;
            float _timer = 0.0f;
            while (_timer <= _duration)
            {
                source.volume = Mathf.Lerp(_initial, _target, _timer / _duration);
                _timer += Time.deltaTime;
                yield return null;
            }
        }
        //CheckLoop        
        if (command.loopCfg.active)
        {
            source.loop = true;
        }
        else
        {
            float _duration = source.clip.length;
            float _timer = 0.0f;
            while (_timer <= _duration)
            {                
                _timer += Time.deltaTime;
                yield return null;
            }
            Stop();
        }
        isPlayProcess = false;
        
    }
    void StartFadeOutProcess(float fadeOutDuration)
    {
        StartCoroutine(IE_FadeOutProcess(fadeOutDuration));
    }

    IEnumerator IE_FadeOutProcess(float fadeOutDuration)
    {
        isFadeProcess = true;
        float _timer = 0.0f;
        while (_timer <= fadeOutDuration)
        {
            float _initial = volume;
            float _target = 0;
            source.volume = Mathf.Lerp(_initial, _target, _timer / fadeOutDuration);
            _timer += Time.deltaTime;
            yield return null;
        }
        ResetAudioSource();
        isFadeProcess = false;
    }
    #endregion

    #region Private - Reset
    void ResetAudioSource()
    {
        source.loop = false;
        source.playOnAwake = false;
        source.volume = 1f;
        source.clip = null;
        source.outputAudioMixerGroup = null;
    }
    #endregion
}