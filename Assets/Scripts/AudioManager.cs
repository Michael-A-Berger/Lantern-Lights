using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Public Properties
    [Header("General Variables")]
    public string clipToPlayOnAwake = string.Empty;
    [Header("Audio Clips")]
    [Space(10)]
    public List<AudioContainer> audioSources;

    // Private Properties
    private Dictionary<string, int> listCipher = new Dictionary<string, int>();


    // Awake()
    private void Awake()
    {
        AudioManager mainMng = this;
        AudioManager[] managerArray = FindObjectsOfType<AudioManager>();
        if (managerArray.Length > 1)
        {
            foreach (AudioManager mng in managerArray)
                if (mng.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    mainMng = mng;
            foreach (AudioContainer container in audioSources)
                StartCoroutine(mainMng.DelayedStop(container.name));
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (clipToPlayOnAwake != string.Empty)
            StartCoroutine(mainMng.DelayedPlay(clipToPlayOnAwake));
    }

    // Start()
    void Start()
    {
        for (int num = 0; num < audioSources.Count; num++)
        {
            audioSources[num].source = gameObject.AddComponent<AudioSource>();
            AudioContainer container = audioSources[num];
            container.source.clip = container.clip;
            container.source.volume = container.volume;
            container.source.loop = container.repeating;
            if (container.startOnSceneLoad)
                container.source.Play();
            listCipher[container.name] = num;
        }
    }

    // DelayedPlay()
    public IEnumerator DelayedPlay(string audioName)
    {
        yield return new WaitForEndOfFrame();
        PlayAudio(audioName);
    }

    // DelayedStop()
    public IEnumerator DelayedStop(string audioName)
    {
        yield return new WaitForEndOfFrame();
        StopAudio(audioName);
    }

    // PlayAudio()
    public void PlayAudio(string audioName)
    {
        if (!listCipher.ContainsKey(audioName))
        {
            Debug.LogError("\t[ AudioManager ] could not find container with name \"" + audioName + "\" !");
        }
        else
        {
            int index = listCipher[audioName];
            if (audioSources[index].source.isPlaying)
                audioSources[index].source.Stop();
            audioSources[index].source.Play();
        }
    }

    // StopAudio()
    public void StopAudio(string audioName)
    {
        if (!listCipher.ContainsKey(audioName))
        {
            Debug.LogError("\t[ AudioManager ] could not find container with name \"" + audioName + "\" !");
        }
        else
        {
            int index = listCipher[audioName];
            if (audioSources[index].source.isPlaying)
                audioSources[index].source.Stop();
        }
    }

    // IsPlaying()
    public bool IsPlaying(string audioName)
    {
        bool isPlaying = false;

        if (!listCipher.ContainsKey(audioName))
        {
            Debug.LogError("\t[ AudioManager ] could not find container with name \"" + audioName + "\" !");
        }
        else
        {
            int index = listCipher[audioName];
            isPlaying = audioSources[index].source.isPlaying;
        }

        return isPlaying;
    }

    // SetVolume()
    public void ResetVolume(float percentage)
    {
        foreach (AudioContainer container in audioSources)
            container.source.volume = container.volume * (percentage / 100);
    }
}

[System.Serializable]
public class AudioContainer
{
    // Public Properties
    public string name = "";
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;
    public bool repeating = false;
    public bool startOnSceneLoad = false;
    [HideInInspector]
    public AudioSource source;
}