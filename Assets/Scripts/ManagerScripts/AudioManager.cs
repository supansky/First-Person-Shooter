using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource music1Source;
    [SerializeField] AudioSource music2Source;

    private AudioSource activeMusic;
    private AudioSource inactiveMusic;

    public float crossFadeRate = 1.5f;
    private bool crossFading;

    [SerializeField] string introBGM;
    [SerializeField] string levelBGM;

    private float musicVolume;
    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = value;

            if (music1Source != null && !crossFading)
            {
                music1Source.volume = musicVolume;
                music2Source.volume = musicVolume;
            }
        }
    }
    public bool MusicMute
    {
        get
        {
            if(music1Source != null)
            {
                return music1Source.mute;
            }
            return false;
        }
        set
        {
            if (music1Source != null)
                music1Source.mute = value;
                music2Source.mute = value;
        }
    }

    public ManagerStatus Status { get; private set; }

    private NetworkService network;

    public float SoundVolume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }
    public bool SoundMute
    {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }
    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }
    private void PlayMusic(AudioClip clip)
    {
        if (crossFading) { return; }
        StartCoroutine(CrossFadeMusic(clip));
    }

    private IEnumerator CrossFadeMusic(AudioClip clip)
    {
        crossFading = true;

        inactiveMusic.clip = clip;
        inactiveMusic.volume = 0;
        inactiveMusic.Play();

        float scaledRate = crossFadeRate * MusicVolume;
        while(activeMusic.volume > 0)
        {
            activeMusic.volume -= scaledRate * Time.deltaTime;
            inactiveMusic.volume += scaledRate * Time.deltaTime;

            yield return null;
        }

        AudioSource temp = activeMusic;

        activeMusic = inactiveMusic;
        activeMusic.volume = MusicVolume;

        inactiveMusic = temp;
        inactiveMusic.Stop();

        crossFading = false;
    }

    public void StopMusic()
    {
        activeMusic.Stop();
        inactiveMusic.Stop();
    }
    public void PlayIntroMusic()
    {
        PlayMusic(Resources.Load($"Music/{introBGM}") as AudioClip);
    }
    public void PlayLevelMusic()
    {
        PlayMusic(Resources.Load($"Music/{levelBGM}") as AudioClip);
    }

    public void Startup(NetworkService service)
    {
        Debug.Log("Audio manager starting...");

        network = service;

        music1Source.ignoreListenerVolume = true;
        music2Source.ignoreListenerVolume = true;
        music1Source.ignoreListenerPause = true;
        music2Source.ignoreListenerPause = true;

        SoundVolume = 1f;
        MusicVolume = 1f;

        activeMusic = music1Source;
        inactiveMusic = music2Source;

        Status = ManagerStatus.Started;
    }
}
