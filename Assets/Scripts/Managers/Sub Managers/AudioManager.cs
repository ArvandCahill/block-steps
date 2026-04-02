using UnityEngine;
using UnityEngine.Audio;
using System;
using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class AudioManager
{
    #region VARIABLES

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup audioMixerGroup;

    [Header("BGM Settings")]
    public AudioSource bgmSource;
    public List<AudioClip> bgmList;
    public int currentBgmIndex;
    public float fadeDuration = 1f;
    public string bgmMixerParam = "BGM_VOLUME";
    private Coroutine bgmFadeRoutine;

    [Header("SFX Settings")]
    public AudioSource sfxSource;
    public string sfxMixerParam = "SFX_VOLUME";
    public float minVolume = 0.4f;
    public float maxVolume = 1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.2f;

    [SerializedDictionary("SFX Name", "Audio Clip")]
    public SerializedDictionary<string, AudioClip> sfxList = new SerializedDictionary<string, AudioClip>();

    [Header("3D SFX Settings")]
    public GameObject sfx3DPrefab;
    public float spatialBlend = 1f;
    public float minDistance = 1f;
    public float maxDistance = 15f;

    [Header("3D SFX Pool")]
    public int poolSize = 15;

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    [HideInInspector] public float savedBgmVolume = 1f;
    [HideInInspector] public float savedSfxVolume = 1f;

    #endregion

    #region INITIALIZE

    public void Initialize(bool bgmOn, bool sfxOn)
    {
        SetBgmActive(bgmOn);
        SetSfxActive(sfxOn);

        SetAudioMixerValue();

        InitializePool();

        if (bgmOn && bgmList.Count > 0)
            PlayBGM(0);
    }

    private void InitializePool()
    {
        if (sfx3DPrefab == null) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(sfx3DPrefab);
            obj.SetActive(false);

            AudioSource source = obj.GetComponent<AudioSource>();
            source.outputAudioMixerGroup = audioMixerGroup;

            sfxPool.Enqueue(source);
        }
    }

    #endregion

    #region BGM

    public void PlayBGM(int index)
    {
        if (bgmSource.clip == bgmList[index] && bgmSource.isPlaying)
            return;

        if (bgmFadeRoutine != null)
            GameManager.instance.StopCoroutine(bgmFadeRoutine);

        bgmFadeRoutine = GameManager.instance.StartCoroutine(FadeBGM(index));

        Debug.Log($"Playing BGM: {bgmSource.clip.name}");
    }

    private IEnumerator FadeBGM(int newIndex)
    {
        float startVolume = bgmSource.volume;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();

        currentBgmIndex = newIndex;
        bgmSource.clip = bgmList[newIndex];
        bgmSource.loop = true;
        bgmSource.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        bgmSource.volume = 1f;
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    #endregion

    #region SFX 2D

    public void PlaySFX(string sfxName)
    {
        if (sfxList.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayRandomSFX(params string[] sfxNames)
    {
        List<AudioClip> candidates = new List<AudioClip>();

        foreach (string name in sfxNames)
        {
            if (sfxList.TryGetValue(name, out AudioClip clip) && clip != null)
                candidates.Add(clip);
        }

        if (candidates.Count > 0)
        {
            AudioClip chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            sfxSource.PlayOneShot(chosen);
        }
    }

    #endregion

    #region SFX 3D (POOLING)

    private AudioSource GetPooledSource()
    {
        if (sfxPool.Count > 0)
        {
            AudioSource source = sfxPool.Dequeue();
            source.gameObject.SetActive(true);

            source.pitch = 1f;
            source.volume = 1f;

            return source;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(sfx3DPrefab);
            return obj.GetComponent<AudioSource>();
        }
    }

    private void ReturnToPool(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
        sfxPool.Enqueue(source);
    }

    private IEnumerator ReturnAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(source);
    }

    public void PlaySFXAtPosition(string sfxName, Vector3 position)
    {
        if (!sfxList.TryGetValue(sfxName, out AudioClip clip))
        {
            Debug.LogWarning($"SFX '{sfxName}' tidak ditemukan!");
            return;
        }

        AudioSource source = GetPooledSource();

        source.transform.position = position;
        source.clip = clip;

        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.outputAudioMixerGroup = audioMixerGroup;

        source.Play();

        GameManager.instance.StartCoroutine(ReturnAfterPlay(source, clip.length));
    }

    public void PlaySFXAtPositionWithVelocity(string sfxName, Vector3 position, float velocity, float maxVelocity = 10f)
    {
        if (velocity < 1f) return;

        if (!sfxList.TryGetValue(sfxName, out AudioClip clip))
            return;

        float t = Mathf.Clamp01(velocity / maxVelocity);

        AudioSource source = GetPooledSource();

        source.transform.position = position;
        source.clip = clip;

        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.volume = Mathf.Lerp(minVolume, maxVolume, t);
        source.pitch = Mathf.Lerp(minPitch, maxPitch, t);

        source.outputAudioMixerGroup = audioMixerGroup;

        source.Play();

        GameManager.instance.StartCoroutine(ReturnAfterPlay(source, clip.length));
    }

    #endregion

    #region VOLUME

    private void SetAudioMixerValue()
    {
        audioMixer?.SetFloat(bgmMixerParam, Mathf.Log10(Mathf.Clamp(savedBgmVolume, 0.0001f, 1f)) * 20f);
        audioMixer?.SetFloat(sfxMixerParam, Mathf.Log10(Mathf.Clamp(savedSfxVolume, 0.0001f, 1f)) * 20f);
    }

    public void SetBgmVolume(float volume)
    {
        savedBgmVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(bgmMixerParam, Mathf.Log10(savedBgmVolume) * 20f);
    }

    public void SetSfxVolume(float volume)
    {
        savedSfxVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(sfxMixerParam, Mathf.Log10(savedSfxVolume) * 20f);
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

    #endregion
}