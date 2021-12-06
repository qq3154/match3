using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioCommand", menuName = "GameConfiguration/Audio/AudioCommand")]
public class AudioCommand : ScriptableObject
{
	#region Config
	[Header("For AudioPlayer")]
	public BasicSetting basicCfg;
	public LoopSetting loopCfg;
	public RandomSetting randomCfg;
	public StartSetting startCfg;

	[Header("For AudioManager")]
	public LimitSetting limitCfg;
	//public LimitPerFrameSetting limitPerFrameCfg;
	public StopOtherSetting stopOtherCfg;
	public StopBgmSetting stopBgmCfg;
	public StopAllSetting stopAllCfg;

	[Header("Mute/Unmute")]
	public MuteSetting muteCfg;
    #endregion


    #region Getting
	public void Excute()
    {
		if (AudioManager.HasInstance())
		{
			AudioManager.instance.Play(this);
		}
		else
		{
			Debug.LogWarning("No AudioManager instance on scene. Drag one to the scene");
		}
	}
	public AudioClip GetClipForPlay()
	{
		// active random clip
		if (randomCfg.active)
		{
			var randomIndex = Random.Range(0, randomCfg.randomClips.Length);
			return randomCfg.randomClips[randomIndex];
		}
		// normal
		return basicCfg.clip;
	}
	#endregion

	#region Class
	//----------AudioPlayer
	[System.Serializable]
	public struct BasicSetting
    {
		public bool activePlayAudio;
		public AudioClip clip;
		public AudioClipType clipType;
		public AudioMixerGroup mixerGroup;
	}

	[System.Serializable]
	public struct LoopSetting
	{
		public bool active;
		/// If we only want it to play a limit loop, set it here. 0 = infinite loop
		public int limitLoopCount;
	}

	[System.Serializable]
	public struct RandomSetting
	{
		public bool active;
		public AudioClip[] randomClips;
	}
	[System.Serializable]
	public struct StartSetting
	{
		public float delay;
		public float fadeInDuration;
	}

	//---------AudioManager
	[System.Serializable]
	public struct LimitSetting
	{
		public int maxInstancePertime;
	}

	[System.Serializable]
	public struct StopOtherSetting
	{
		public bool active;
		public AudioCommand[] others;
		public float fadeOutDuration;
	}

	[System.Serializable]
	public struct StopBgmSetting
	{
		public bool active;
		public float fadeOutDuration;
	}

	[System.Serializable]
	public struct StopAllSetting
	{
		public bool active;
		public float fadeOutDuration;
	}

	[System.Serializable]
	public struct MuteSetting
	{
		public bool active;
		public bool muteAll;
	}
	#endregion
}
