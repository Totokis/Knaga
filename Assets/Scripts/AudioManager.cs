using UnityEngine;
using Assets.Scripts;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            PlayerPrefs.SetFloat("sfxVolume", 1);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        
        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.Clip;

            s.source.volume = s.Volume == 0f ? 1 : s.Volume;
            s.source.pitch = s.Pitch == 0.1f ? 1 : s.Pitch;
        }

    }

    public void Play(String name)
    {
        Single sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        Sound sound = Array.Find(Sounds, s => s.Name == name);
        if (sound == null)
        {
            Debug.LogWarning($"Probowales odtworzyc dzwiek o nazwie \"{name}\" ktory nie istnieje");
            return;
        }
        //Debug.Log("normal volume " + sound.source.volume);
        //sound.source.volume *= sfxVolume;
        //Debug.Log("changed volume " + sound.source.volume);

        sound.source.volume = sound.Volume * sfxVolume;

        sound.source.Play();
    }

    public void PlayWithRandomPitch(Single pitchMin, Single pitchMax, String name)
    {
        Sound sound = Array.Find(Sounds, s => s.Name == name);
        if (sound == null)
        {
            Debug.LogWarning($"Probowales odtworzyc dzwiek o nazwie \"{name}\" ktory nie istnieje");
            return;
        }
        sound.source.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
        sound.source.Play();
    }
}
