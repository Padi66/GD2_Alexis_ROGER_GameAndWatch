using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    None,
    ObjectMovement,
    PlayerMovement,
    Destruction,
    Death,
    Win,
    GameStart,
    ButtonClick,
    TouchObject,
    BackgroundMusic,
    GameOver,
    HackNodeNormal,
    HackNodeBonus,
    HackNodeMalus,
    HackNodeGoal,
    BossExplosion,
    BossFall
}

[System.Serializable]
public struct AudioInfos
{
    public AudioType audioType;
    public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "AudioEventDispatcher", menuName = "Scriptable Objects/AudioEventDispatcher")]
public class AudioEventDispatcher : ScriptableObject
{
    [SerializeField] private AudioInfos[] audioClips;

    public event Action<AudioClip> OnAudioEvent;
    public event Action<AudioClip> OnMusicEvent;

    public void PlayAudio(AudioType audioType)
    {
        for (int i = 0; i < audioClips.Length; i++)
        {
            if (audioClips[i].audioType == audioType)
            {
                if (audioType == AudioType.BackgroundMusic)
                {
                    OnMusicEvent?.Invoke(audioClips[i].audioClip);
                }
                else
                {
                    OnAudioEvent?.Invoke(audioClips[i].audioClip);
                }
                return;
            }
        }
    }
}