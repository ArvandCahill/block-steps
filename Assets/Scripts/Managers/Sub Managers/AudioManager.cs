using UnityEngine;
using UnityEngine.Audio;
using System;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

[Serializable]
public class AudioManager
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup audioMixerGroup;

    [Header("BGM Settings")]
    public AudioSource bgmSource;
    public List<AudioClip> bgmList;
    public int currentBgmIndex;
    public string bgmMixerParam = "BGM_VOLUME";

    [Header("SFX Settings")]
    public AudioSource sfxSource;
    public string sfxMixerParam = "SFX_VOLUME";
    public float minVolume = 0.4f;
    public float maxVolume = 1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.2f;

    [SerializedDictionary("SFX Name", "Audio Clip")]
    public SerializedDictionary<string, AudioClip> sfxList = new SerializedDictionary<string, AudioClip>();

    [HideInInspector] public float savedVolume;
    [HideInInspector] public float savedBgmVolume;
    [HideInInspector] public float savedSfxVolume;

    public void Initialize(bool bgmOn, bool sfxOn)
    {
        SetBgmActive(bgmOn);
        SetSfxActive(sfxOn);

        SetAudioMixerValue();

        if (bgmOn && bgmList.Count > 0)
            PlayBGM(0);
    }


    public void PlayBGM(int index)
    {
        if (bgmSource.isPlaying && currentBgmIndex == index && bgmSource.clip == bgmList[index])
        {
            Debug.Log($"BGM '{bgmList[index].name}' BGM Already Played, Skip.");
            return;
        }

        currentBgmIndex = index;
        bgmSource.clip = bgmList[index];
        bgmSource.loop = true;
        bgmSource.Play();

        Debug.Log($"Now playing BGM: {bgmList[index].name}");
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            Debug.Log("BGM Stopped");
        }
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxList.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{sfxName}' tidak ditemukan!");
        }
    }

    public void PlaySFXWithVelocity(string sfxName, float velocity, float maxVelocity = 10f)
    {
        if (velocity < 1f) return;
        if (sfxList.TryGetValue(sfxName, out AudioClip clip))
        {
            float t = Mathf.Clamp01(velocity / maxVelocity);

            sfxSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
            sfxSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);

            sfxSource.clip = clip;
            sfxSource.Play();
            Debug.Log($"Play SFX '{sfxName}' with velocity {velocity}");
        }
        else
        {
            Debug.LogWarning($"SFX '{sfxName}' tidak ditemukan!");
        }
    }

    public void PlayRandomSFX(params string[] sfxNames)
    {
        if (sfxNames == null || sfxNames.Length == 0)
        {
            Debug.LogWarning("sfx names empty");
            return;
        }

        List<AudioClip> candidates = new List<AudioClip>();

        foreach (string name in sfxNames)
        {
            if (sfxList.TryGetValue(name, out AudioClip clip) && clip != null)
            {
                candidates.Add(clip);
            }
        }

        if (candidates.Count > 0)
        {
            AudioClip chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            sfxSource.PlayOneShot(chosen);
        }
        else
        {
            Debug.LogWarning("no valid audioclips");
        }
    }

    public void AssignAudioMixer(AudioSource[] allAudioSource)
    {
        foreach (AudioSource audioSource in allAudioSource)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
    }

    private void SetAudioMixerValue()
    {
        audioMixer?.SetFloat(bgmMixerParam, Mathf.Log10(Mathf.Clamp(savedBgmVolume, 0.0001f, 1f)) * 20f);
        audioMixer?.SetFloat(sfxMixerParam, Mathf.Log10(Mathf.Clamp(savedSfxVolume, 0.0001f, 1f)) * 20f);
    }

    public void SetBgmVolume(float volume)
    {
        savedBgmVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(bgmMixerParam, Mathf.Log10(Mathf.Clamp(savedBgmVolume, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat("BgmVolume", savedBgmVolume);
    }

    public void SetSfxVolume(float volume)
    {
        savedSfxVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(sfxMixerParam, Mathf.Log10(Mathf.Clamp(savedSfxVolume, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat("SfxVolume", savedSfxVolume);
    }

    public void SetBgmActive(bool active)
    {
        if (bgmSource != null)
            bgmSource.mute = !active;
    }

    public void SetSfxActive(bool active)
    {
        if (sfxSource != null)
            sfxSource.mute = !active;
    }

    public AudioClip GetAudioByName(AudioClip[] audioArray, string name)
    {
        foreach (var clip in audioArray)
        {
            if (clip != null && clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
}
