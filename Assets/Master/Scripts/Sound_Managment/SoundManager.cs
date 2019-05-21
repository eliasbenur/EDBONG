using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{

    public enum Sound
    {
        PlayerFTS,
        PlayerGetHit,
        PlayerFalling,
        PlayerDash,
        SpydieNoise,
        SpydieMeleAtack,
        PlayerSlicing,
        VoiceAlarm,
        BossCombat,
        DoorOpening_Closing,
        Kamikaze,
        BossShoot,
        BossExplode,
        BossLaser
    }

    private static Dictionary<Sound, float> soundTimerDictionary;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerFTS] = 0f;
        soundTimerDictionary[Sound.PlayerFalling] = 0f;
        soundTimerDictionary[Sound.Kamikaze] = 0f;
        soundTimerDictionary[Sound.BossShoot] = 0f;
        soundTimerDictionary[Sound.BossExplode] = 0f;
        soundTimerDictionary[Sound.SpydieMeleAtack] = 0f;
        soundTimerDictionary[Sound.BossLaser] = 0f;
    }

    public static void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.volume = GetAudioVolume(sound);
            audioSource.Play();
        }
    }

    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.volume = GetAudioVolume(sound);
            audioSource.Play();
        }
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.PlayerFalling:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 2f; //DELAY
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.PlayerFTS:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .5f; //DELAY
                    if(lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            
            case Sound.Kamikaze:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 4.7f; //DELAY
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case Sound.SpydieMeleAtack:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 1f; //DELAY
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case Sound.BossShoot:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 1.3f; //DELAY
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.BossExplode:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 1f; //DELAY
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.BossLaser:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 5f; //DELAY
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            default:
                return true;
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }

    private static float GetAudioVolume(Sound sound)
    {
        foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.volume;
            }
        }
        return 1;
    }
}
