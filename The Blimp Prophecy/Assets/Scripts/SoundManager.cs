using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AudioClipID
{
	BGM_MAIN_MENU = 0,
	BGM_GAME_LEVEL = 1,

	SFX_ALARM = 10,
	SFX_GAS = 11,
	SFX_TRAP = 12,

	SFX_BUTTON_CLICK = 20,
	SFX_COMPUTER_ACTIVATED = 21,
    SFX_DOOR_OPEN = 22,
    SFX_ALARM_TRIGGERRED = 23,

    SFX_PLAYER_WIN = 30,
    SFX_PLAYER_WALKING = 31,

	TOTAL = 200
}


[System.Serializable]
public class AudioClassInfo
{
	public AudioClipID audioClipID;
	public AudioClip audioClip;
}

public class SoundManager : MonoBehaviour {

	private static SoundManager mInstance;

	public static SoundManager Instance
	{
		get
		{
			if (mInstance == null) 
			{
				GameObject tempObject = GameObject.FindWithTag("SoundManager");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("SoundManager");
					mInstance = obj.AddComponent<SoundManager>();
					obj.tag = "SoundManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<SoundManager>();
				}
				DontDestroyOnLoad(mInstance.gameObject);
			}
			return mInstance;
		}
	}

	public static bool CheckInstanceExist()
	{
		return mInstance;
	}

	public List<AudioClassInfo> audioClipInfo = new List <AudioClassInfo>();

	public float bgmVolume = 1.0f;
	public float sfxVolume = 1.0f;

	public bool isCoroutineOn;

	public AudioSource bgmAudioSource;
	public AudioSource sfxAudioSource;

	public List<AudioSource> loopingSFXAudioSourceList = new List<AudioSource>();

	void Awake () 
	{
		if(SoundManager.CheckInstanceExist())
		{
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		AudioSource[] audioSourceList = this.GetComponentsInChildren<AudioSource>();

		if(audioSourceList[0].gameObject.name == "BGMAudioSource")
		{
			bgmAudioSource = audioSourceList[0];
			sfxAudioSource = audioSourceList[1];
		}
		else 
		{
			bgmAudioSource = audioSourceList[1];
			sfxAudioSource = audioSourceList[0];
		}
	}

	AudioClip FindAudioClip(AudioClipID audioClipID)
	{
		for(int i = 0; i < audioClipInfo.Count; i++)
		{
			if(audioClipInfo[i].audioClipID == audioClipID)
			{
				return audioClipInfo[i].audioClip;
			}
		}

		Debug.LogError("Cannot find ID: " + audioClipID);

		return null;
	}

	public AudioClip FindSFXClip(AudioClipID audioClipID)
	{
		for(int i = 0; i < audioClipInfo.Count; i++)
		{
			if(audioClipInfo[i].audioClipID == audioClipID)
			{
				return audioClipInfo[i].audioClip;
			}
		}

		Debug.LogError("Cannot find ID: " + audioClipID);

		return null;
	}

	public void PlayBGM(AudioClipID audioClipID)
	{
		if(!bgmAudioSource.isPlaying)
		{
			bgmAudioSource.clip = FindAudioClip(audioClipID);
			bgmAudioSource.volume = bgmVolume;
			bgmAudioSource.loop = true;
			bgmAudioSource.Play();
		}
			
	}

	public void FadeInPlayBGM(AudioClipID audioClipID)
	{
		if(!bgmAudioSource.isPlaying)
		{
			bgmAudioSource.clip = FindAudioClip(audioClipID);
			bgmAudioSource.loop = true;
			StartCoroutine(FadeIn(bgmAudioSource, bgmAudioSource.clip, 0.5f, 1f));
		}
	}

	public void FadeOutBGM()
	{
		StartCoroutine(FadeOut(bgmAudioSource, 0.5f));
	}

	public void PauseBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Pause();
		}
	}

	public void StopBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Stop();
		}
	}

	//public void PlaySFX(AudioClipID audioClipID)
	public void PlaySFX(AudioClipID audioClipID)
	{
		sfxAudioSource.PlayOneShot(FindSFXClip(audioClipID), sfxVolume);
	}

	public void PlayLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPlay = FindSFXClip(audioClipID);

		if(clipToPlay == null)
		{
			return;
		}

		for(int i=0; i<loopingSFXAudioSourceList.Count; i++)
		{
			if(loopingSFXAudioSourceList[i].clip == clipToPlay)
			{
				if(loopingSFXAudioSourceList[i].isPlaying)
				{
					return;
				}
				else 
				{
					loopingSFXAudioSourceList[i].Play();
				}

				loopingSFXAudioSourceList[i].volume = sfxVolume;
				return;
			}
		}

		AudioSource newInstance = sfxAudioSource.gameObject.AddComponent<AudioSource>();
		newInstance.playOnAwake = false;
		newInstance.clip = clipToPlay;
		newInstance.volume = sfxVolume;
		newInstance.loop = true;
		newInstance.Play();
		loopingSFXAudioSourceList.Add(newInstance);
	}

	public void PlayLoopingSFXWithFadeIn(AudioClipID audioClipID, float fadeInDuration)
	{
		AudioClip clipToPlay = FindAudioClip(audioClipID);

		if(clipToPlay == null)
		{
			return;
		}

		for(int i=0; i<loopingSFXAudioSourceList.Count; i++)
		{
			if(loopingSFXAudioSourceList[i].clip == clipToPlay)
			{
				if(loopingSFXAudioSourceList[i].isPlaying)
				{
					return;
				}

				loopingSFXAudioSourceList[i].volume = sfxVolume;
				StartCoroutine(FadeIn(loopingSFXAudioSourceList[i], clipToPlay, fadeInDuration, sfxVolume));
				return;
			}
		}

		AudioSource newInstance = sfxAudioSource.gameObject.AddComponent<AudioSource>();
		newInstance.playOnAwake = false;
		newInstance.loop = true;

		StartCoroutine(FadeIn(newInstance, clipToPlay, fadeInDuration, sfxVolume));
		loopingSFXAudioSourceList.Add(newInstance);
	}

	public void PauseLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPause = FindAudioClip(audioClipID);

		if(clipToPause == null)
		{
			return;
		}

		for(int i=0; i<loopingSFXAudioSourceList.Count; i++)
		{
			if(loopingSFXAudioSourceList[i].clip == clipToPause)
			{
				loopingSFXAudioSourceList[i].Pause();
				return;
			}
		}
	}

	public void StopLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		if(clipToStop == null)
		{
			return;
		}

		for(int i=0; i<loopingSFXAudioSourceList.Count; i++)
		{
			if(loopingSFXAudioSourceList[i].clip == clipToStop)
			{
				loopingSFXAudioSourceList[i].Stop();
				return;
			}
		}
	}
		

	public IEnumerator FadeOutBGMRoutine()
	{
		while(bgmAudioSource.volume > 0)
		{
			bgmAudioSource.volume -= Time.deltaTime / 1f;
			yield return null;
		}

		StopBGM();
	}

	public IEnumerator FadeInBGMRoutine()
	{
		while(bgmAudioSource.volume <= bgmVolume)
		{
			bgmAudioSource.volume += Time.deltaTime / 1f;
			yield return null;
		}
	}

	IEnumerator FadeOut(AudioSource audioSource, float fadeOutDuration)
	{
		float fadeOutTimer = 0.0f;
		float fadeOutSpeed = audioSource.volume / fadeOutDuration * Time.deltaTime;;

		while(fadeOutTimer < fadeOutDuration)
		{
			fadeOutTimer += Time.deltaTime;
			audioSource.volume -= fadeOutSpeed;
			yield return null;
		}
		audioSource.volume = 0.0f;
		audioSource.Stop();
	}

	IEnumerator FadeOutIn(AudioSource audioSource, AudioClip audioClip, float fadeOutDuration, float fadeInDuration, float maxVolume)
	{
		float fadeOutTimer = 0.0f;
		float originalVolume = audioSource.volume;
		float fadeOutSpeed = originalVolume / fadeOutDuration * Time.deltaTime;

		while(fadeOutTimer < fadeOutDuration)
		{
			fadeOutTimer += Time.deltaTime;
			audioSource.volume -= fadeOutSpeed;
			yield return null;
		}
		StartCoroutine(FadeIn(audioSource, audioClip, fadeInDuration, maxVolume));
	}

	IEnumerator FadeIn(AudioSource audioSource, AudioClip audioClip, float fadeInDuration, float maxVolume)
	{
		audioSource.clip = audioClip;
		audioSource.volume = 0.0f;
		audioSource.Play();

		float fadeInTimer = 0.0f;
		float fadeInSpeed = maxVolume / fadeInDuration * Time.deltaTime;

		while(fadeInTimer < fadeInDuration)
		{
			fadeInTimer += Time.deltaTime;
			audioSource.volume += fadeInSpeed;
			yield return null;
		}
		audioSource.volume = maxVolume;
	}

	IEnumerator FadeOutAll(List<AudioSource> audioSourceList, float fadeOutDuration)
	{
		float fadeOutTimer = 0.0f;
		List<float> fadeOutSpeedList = new List<float>();

		for(int i=0; i<audioSourceList.Count; i++)
		{
			fadeOutSpeedList.Add(audioSourceList[i].volume / fadeOutDuration * Time.deltaTime);
		}

		while(fadeOutTimer < fadeOutDuration)
		{
			fadeOutTimer += Time.deltaTime;
			for(int i=0; i<audioSourceList.Count; i++)
			{
				audioSourceList[i].volume -= fadeOutSpeedList[i];
			}
			yield return null;
		}
		for(int i=0; i<audioSourceList.Count; i++)
		{
			audioSourceList[i].volume = 0.0f;
			audioSourceList[i].Stop();
		}
	}

	IEnumerator FadeOutInAll(List<AudioSource> audioSourceList, float fadeOutDuration, float fadeInDuration)
	{
		float fadeOutTimer = 0.0f;
		List<float> fadeOutSpeedList = new List<float>();
		List<float> maxVolumeList = new List<float>();

		for(int i=0; i<audioSourceList.Count; i++)
		{
			fadeOutSpeedList.Add(audioSourceList[i].volume / fadeOutDuration * Time.deltaTime);
			maxVolumeList.Add(audioSourceList[i].volume);
		}

		while(fadeOutTimer < fadeOutDuration)
		{
			fadeOutTimer += Time.deltaTime;
			for(int i=0; i<audioSourceList.Count; i++)
			{
				audioSourceList[i].volume -= fadeOutSpeedList[i];
			}
			yield return null;
		}
		StartCoroutine(FadeInAll(audioSourceList, fadeInDuration, maxVolumeList));
	}

	IEnumerator FadeInAll(List<AudioSource> audioSourceList, float fadeInDuration, List<float> maxVolumeList)
	{
		float fadeInTimer = 0.0f;
		List<float> fadeInSpeedList = new List<float>();

		for(int i=0; i<audioSourceList.Count; i++)
		{
			audioSourceList[i].volume = 0.0f;
			audioSourceList[i].Play();
			fadeInSpeedList.Add(maxVolumeList[i] / fadeInDuration * Time.deltaTime);
		}

		while(fadeInTimer < fadeInDuration)
		{
			fadeInTimer += Time.deltaTime;
			for(int i=0; i<audioSourceList.Count; i++)
			{				
				audioSourceList[i].volume += fadeInSpeedList[i];
			}
			yield return null;
		}
		for(int i=0; i<audioSourceList.Count; i++)
		{
			audioSourceList[i].volume = maxVolumeList[i];
		}
	}
}
