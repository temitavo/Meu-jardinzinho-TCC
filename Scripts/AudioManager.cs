using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if(instance != null){
            Debug.LogWarning("Found more than one Audio Manager in the scene.");
            Destroy(gameObject);
            return;
        }
        instance = this;

        //DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds){

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.group;

        }
        
    }

    void Start(){

        Play("Theme");

    }

    public static AudioManager GetInstance(){
        return instance;
    }

    public void Play(string name){

        Sound s = Array.Find(sounds, sound => sound.name == name);

        if(s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();

    }
}
