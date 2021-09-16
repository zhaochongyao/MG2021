using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioMgr : BaseManager<AudioMgr>
{
    private AudioSource bkSource;
    private float bkVolume = 1.0f;

    private GameObject soundObj = null;     //用来挂载AudioSource
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundVolume = 1.0f;
    public AudioMgr()
    {
        MonoManager.GetInstance().AddUpdateListener(UpdateAudio);
    }
    public void UpdateAudio()
    {
        for (int i = soundList.Count - 1; i >= 0; i++)       //从后往前遍历,若从前往后因为RemoveAt会更新数组的index所以会漏掉元素
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 播放背景音乐 从Music/BkMusic/name 路径读取
    /// </summary>
    /// <param name="name"></param>
    public void PlayBKAudio(string name)
    {
        if (bkSource == null)
        {
            soundObj = new GameObject("SoundController");
            bkSource = soundObj.AddComponent<AudioSource>();
            bkSource.name = "backgroundAudio";
        }
        ResourceMgr.GetInstance().LoadAsyn<AudioClip>("Music/BkMusic" + name, (clip) =>
          {
              bkSource.loop = true;
              bkSource.clip = clip;
              bkSource.volume = bkVolume;
              bkSource.Play();
          });
    }
    public void PauseBKAudio()
    {
        if (bkSource == null)
        {
            return;
        }
        bkSource.Pause();
    }
    public void StopBKAudio()
    {
        if (bkSource == null)
        {
            return;
        }
        bkSource.Stop();
    }
    public void changeBkVolume(float volume)
    {
        bkVolume = volume;
        if (bkSource == null)
        {
            return;
        }
        bkSource.volume = bkVolume;
    }
    public void PlaySound(string name,bool isLoop,UnityAction<AudioSource> callback = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject("SoundController");
        }
        ResourceMgr.GetInstance().LoadAsyn<AudioClip>("Music/Sound/"+name, (clip)=> {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = soundVolume;
            source.loop = isLoop;
            source.Play();
            soundList.Add(source);
            if (callback != null)
            {
                callback(source);
            }
        });
    }
    public void StopSound(AudioSource source)
    {
        if (!soundList.Contains(source))
        {
            return;
        }
        soundList.Remove(source);
        source.Stop();
        GameObject.Destroy(source);
    }
    public void changeSoundVolum(float volume)
    {
        soundVolume = volume;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundVolume;
        }
    }
}
