using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum location
{
    none,
    Dungeon,
    restaurant,
}
[System.Serializable]
public struct SfxEntry
{
    public SoundManager.sfx key;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public enum sfx
    {
        Wrong,
        Click,
        FoodMove,
        FoodCompleted,
        Foot,
        money,
        Cooking,
        ItemPickup,
        BardBuff,
        VampireBuff,
        PlayerAttack,
        ElfAttack,
        BardAttack_one,
        BardAttack_two,
        BardAttack_three,
        GirlSound,
        ManSound,
        Enhance,
        PiggyBank,
    }

    public enum bgm
    {
        GameBgm,
    }

    [SerializeField] private location currentLocation = location.none;

    public AudioClip[] bgmClips;

    public float bgmVolume;
    public float reduceSoundSpeed;
    private AudioSource bgmPlayer;


    public SfxEntry[] sfxDungeonEntries;
    public SfxEntry[] sfxRestaurantEntries;
    public SfxEntry[] sfxCommonEntries;

    public float sfxVolume;
    public int channels = 8;
    private AudioSource[] sfxSound;


    static SoundManager instance;
    public static SoundManager GetInstance() => instance;


    private Dictionary<sfx, AudioClip> sfxDungeonDict;
    private Dictionary<sfx, AudioClip> sfxRestaurantDict;
    private Dictionary<sfx, AudioClip> sfxCommonDict;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();

        sfxDungeonDict = ToDict(sfxDungeonEntries);
        sfxRestaurantDict = ToDict(sfxRestaurantEntries);
        sfxCommonDict = ToDict(sfxCommonEntries);
    }

    private Dictionary<sfx, AudioClip> ToDict(SfxEntry[] entries)
    {
        var dict = new Dictionary<sfx, AudioClip>();
        foreach (var e in entries)
            dict[e.key] = e.clip;
        return dict;
    }



    private void Init()
    {
        // 저장된 볼륨값 불러오기 (기존 로직 유지)
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.2f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        // BGM 플레이어 생성
        GameObject bgmObject = new GameObject("BGM");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        // SFX 채널 풀 생성
        GameObject sfxObject = new GameObject("SFX");
        sfxObject.transform.parent = transform;
        sfxSound = new AudioSource[channels];

        for (int i = 0; i < sfxSound.Length; i++)
        {
            sfxSound[i] = sfxObject.AddComponent<AudioSource>();
            sfxSound[i].playOnAwake = false;
            sfxSound[i].volume = sfxVolume;
        }
    }

    public void SetLocation(location loc)
    {
        if (currentLocation == loc) return; // 같은 위치면 무시

        currentLocation = loc;

        All_Sfx_Stop();
    }
    private AudioClip GetSfxClip(sfx id)
    {
        switch (currentLocation)
        {
            case location.Dungeon:
                if (sfxDungeonDict.TryGetValue(id, out var clip) && clip != null)
                    return clip;
                break;
            case location.restaurant:
                if (sfxRestaurantDict.TryGetValue(id, out clip) && clip != null)
                    return clip;
                break;
        }

        // 공통 폴백
        sfxCommonDict.TryGetValue(id, out var commonClip);
        return commonClip;
    }

    public bool AnySfxPlaying(sfx type)
    {
        var clip = GetSfxClip(type);
        if (clip == null) return false;

        foreach (var audio in sfxSound)
        {
            if (audio.clip == clip && audio.isPlaying)
                return true;
        }
        return false;
    }

    public void SfxPlay(sfx id, bool _loopcheck)
    {
        var clip = GetSfxClip(id);
        if (clip == null)
        {
            return;
        }

        for (int i = 0; i < sfxSound.Length; i++)
        {
            if (sfxSound[i].isPlaying) continue;

            sfxSound[i].clip = clip;
            sfxSound[i].volume = sfxVolume; // 내부 설정값 사용
            sfxSound[i].loop = _loopcheck;
            sfxSound[i].Play();
            break;
        }
    }

    public void ChangeLoation()
    {
        // 현재 위치에 따라 BGM과 SFX를 업데이트
        if (currentLocation == location.Dungeon)
        {
            PlayBgm(bgm.GameBgm); // 던전 BGM 재생
        }
        else if (currentLocation == location.restaurant)
        {
            PlayBgm(bgm.GameBgm); // 레스토랑 BGM 재생
        }
        else
        {
            Bgm_Stop(); // 위치가 설정되지 않은 경우 BGM 정지
        }
        UpdateSfxVolumes(); // SFX 볼륨 업데이트
    }


    public void UpdateSfxVolumes()
    {
        foreach (var src in sfxSound)
        {
            if (src != null && src.isPlaying)
                src.volume = sfxVolume;
        }
    }

    public void Sfx_Stop(sfx id)
    {
        var clip = GetSfxClip(id);
        if (clip == null) return;

        for (int i = 0; i < sfxSound.Length; i++)
        {
            if (sfxSound[i].clip == clip)
                sfxSound[i].Stop();
        }
    }

    public void All_Sfx_Stop()
    {
        for (int i = 0; i < sfxSound.Length; i++)
            sfxSound[i].Stop();
    }

    public void PlayBgm(bgm _bgm)
    {
        bgmPlayer.clip = bgmClips[(int)_bgm];
        bgmPlayer.Play();
    }

    public void Bgm_Stop()
    {
        bgmPlayer.Stop();
    }

    public void SetSoundBgm(float volume)
    {
        bgmPlayer.volume = volume;
    }

    public void ReduceSoundBgm()
    {
        if (bgmPlayer.volume >= 0)
            bgmPlayer.volume -= Time.deltaTime * reduceSoundSpeed;
    }
}