using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public Sound[] sounds;
    public static AudioManager Instance;

    private bool _muted;

    void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
		}
	}

    public void Mute()
    {
        _muted = !_muted;
        AudioListener.pause = _muted;
    }
	public void Play(string sound, bool checkIfIsAlreadyPlaying = false)
	{
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
		{
			Debug.LogError("Sound: " + sound + " not found!");
			return;
		}
        if (checkIfIsAlreadyPlaying && s.source.isPlaying)
        {
            return;
        }

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}
    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }
}
