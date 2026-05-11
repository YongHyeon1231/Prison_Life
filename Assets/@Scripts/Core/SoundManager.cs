using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private Dictionary<SoundType, AudioClip> _clipMap;

    private void Awake()
    {
        _clipMap = new Dictionary<SoundType, AudioClip>();
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            var clip = Resources.Load<AudioClip>($"Sound/{type}");
            if (clip != null)
                _clipMap[type] = clip;
            else
                Debug.LogWarning($"[SoundManager] 클립 없음: Resources/Sound/{type}");
        }
    }

    public void Play(SoundType type)
    {
        if (!_clipMap.TryGetValue(type, out AudioClip clip)) return;
        _audioSource.PlayOneShot(clip);
    }

    public void Stop() => _audioSource.Stop();

    public void SetMute(bool mute) => _audioSource.mute = mute;
    public bool IsMuted             => _audioSource.mute;
}
