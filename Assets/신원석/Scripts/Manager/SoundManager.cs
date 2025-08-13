using UnityEngine;
using UnityEngine.Rendering;

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
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }
    public bool AnySfxPlaying(sfx type)
    {
        foreach (var audio in sfxSound)
        {
            if (audio.clip == sfxClips[(int)type] && audio.isPlaying)
                return true;
        }
        return false;
    }
    public void SfxPlay(sfx sfx, bool _loopcheck, float volume = 0.5f)
    {
        if (sfxClips[(int)sfx] == null)
        {
            Debug.LogWarning($"SFX 클립이 없습니다: {sfx}");
            return;
        }

        for (int i = 0; i < sfxSound.Length; i++)
        {
            if (sfxSound[i].isPlaying)
            {
                continue;
            }

            sfxSound[i].clip = sfxClips[(int)sfx];
            sfxSound[i].Play();
            //sfxSound[i].volume = volume > 0 ? volume : sfxVolume;
            //sfxSound[i].volume = volume;
            sfxSound[i].volume = sfxVolume; // 항상 내부 설정값 사용

            if (_loopcheck == true)
            {
                sfxSound[i].loop = true;
            }
            else
            {
                sfxSound[i].loop = false;
            }
            break;
        }

    }

    public void UpdateSfxVolumes()
    {
        foreach (var src in sfxSound)
        {
            if (src != null && src.isPlaying)
                src.volume = sfxVolume;
        }
    }


    public void Sfx_Stop(sfx _sfx)
    {
        for (int i = 0; i < sfxSound.Length; i++)
        {
            if (sfxSound[i].clip == sfxClips[(int)_sfx])
            {
                sfxSound[i].Stop();
            }
        }
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
    public void All_Sfx_Stop()
    {
        for (int i = 0; i < sfxSound.Length; i++)
        {
            sfxSound[i].Stop();
        }
    }
    public static SoundManager GetInstance()
    {
        return instance;
    }
    private void Init()
    {
        // 🔽 저장된 볼륨값 불러오기
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.2f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        GameObject bgmObject = new GameObject("BGM");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
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
    public void SetSoundBgm(float volume)
    {
        bgmPlayer.volume = volume;
    }

    public void ReduceSoundBgm()
    {
        if(bgmPlayer.volume >= 0)
        {
            bgmPlayer.volume -= Time.deltaTime * reduceSoundSpeed;
        }     
    }

    [Header("#BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    AudioSource bgmPlayer;
    public float reduceSoundSpeed;


    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxSound;



    static SoundManager instance;


}
