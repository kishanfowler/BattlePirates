using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MusicPlaylist : MonoBehaviour
{
    public List<AudioClip> musicTracks;
    public AudioMixerGroup outputMixerGroup;

    private AudioSource _audioSource;
    private int _currentTrackIndex = 0;

    void Awake()
    {
        // Only allow one instance to persist
        if (FindObjectsOfType<MusicPlaylist>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        if (outputMixerGroup != null)
            _audioSource.outputAudioMixerGroup = outputMixerGroup;
    }

    void Start()
    {
        PlayTrack(_currentTrackIndex);
    }

    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _currentTrackIndex++;
            if (_currentTrackIndex >= musicTracks.Count)
                _currentTrackIndex = 0; // Restart from beginning

            PlayTrack(_currentTrackIndex);
        }
    }

    void PlayTrack(int index)
    {
        if (musicTracks.Count == 0) return;
        Debug.Log(_audioSource.isPlaying);
        _audioSource.clip = musicTracks[index];
        _audioSource.Play();
    }
}