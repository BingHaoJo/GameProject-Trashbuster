using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Pool")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private bool dontDestroyOnLoad = true;

    private readonly List<AudioSource> pool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        // Build a pool of AudioSources (2D, non-spatial)
        for (int i = 0; i < poolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f; // 2D
            pool.Add(src);
        }
    }

    private AudioSource GetFreeSource()
    {
        // Return any idle source
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isPlaying)
                return pool[i];
        }
        // If all busy, reuse the first one (avoids allocs)
        return pool[0];
    }

    /// <summary>Plays a one-shot 2D SFX using the pool.</summary>
    public void PlaySfx(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) { Debug.LogWarning("AudioManager.PlaySfx: null clip"); return; }

        var src = GetFreeSource();
        src.pitch = pitch;
        src.volume = Mathf.Clamp01(volume);
        // For 2D SFX we don't care about world position; keep it on the manager
        src.PlayOneShot(clip);
    }

    public AudioSource GetAudioSource(AudioClip clip)
    {
        AudioSource gotSrc = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].clip == clip)
                gotSrc = pool[i];
            else if (pool[i].clip == null && gotSrc == null)
                gotSrc = pool[i];
                
        }

        if (gotSrc == null)
        {
            gotSrc = pool[0];
        }
        
        return gotSrc;
    }
}
