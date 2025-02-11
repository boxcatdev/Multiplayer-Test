using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerFX : MonoBehaviour
{
    public static AudioManagerFX Audio;

    private AudioSource audioSource;

    [Header("Audio Settings")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float sfxVolume;
    [SerializeField, Range(0.0f, 1.0f)] private float menuFXVolume;

    [Header("Interface FX Clips")]
    [SerializeField] private AudioClip clickedClip;
    [SerializeField] private AudioClip sliderClip;

    [Header("Combat SFX Clips")]
    [SerializeField] private AudioClip defaultPunchClip;
    [SerializeField] private AudioClip[] normalPunchClips;
    [SerializeField] private AudioClip[] heavyPunchClips;
    [SerializeField] private AudioClip[] deathSFXClips;
    //[SerializeField] private AudioClip tileSelectedClip;
    //[SerializeField] private AudioClip tileDeselectedClip;
    //[SerializeField] private AudioClip successfulCheck;
    //[SerializeField] private AudioClip failedCheck;
    //[SerializeField] private AudioClip gameWinClip;

    [Header("Game SFX Clips")]
    [SerializeField] private AudioClip interactClip;

    public static bool IsMuted { get; private set; }

    public Action OnMuteSettingUpdate;

    private void Awake()
    {
        #region Singleton
        if (Audio != null && Audio != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Audio = this;
        }
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        #endregion

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // change mute setting
        //bool soundOn = PlayerPrefs.GetInt(PrefKeys.SFX) == 0 ? true : false;
        bool soundOn = true;
        SetMuteSetting(!soundOn);
        Debug.Log("[SFX]Initial mute check");
    }

    /*public void PlaySoundEffect(SFXType type)
    {
        AudioClip usedClip = tileSelectedClip;

        switch (type)
        {
            case SFXType.TileSelected:
                usedClip = tileSelectedClip;
                break;
            case SFXType.TileDeselected:
                usedClip = tileDeselectedClip;
                break;
            case SFXType.SuccessfulCheck:
                usedClip = successfulCheck;
                break;
            case SFXType.FailedCheck:
                usedClip = failedCheck;
                break;
        }

        audioSource.PlayOneShot(usedClip, sfxVolume);
    }*/
    public void PlayRandSoundEffect(string effect)
    {
        AudioClip usedClip = defaultPunchClip;

        // randomize pitch
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);


        switch (effect)
        {
            default:
                usedClip = defaultPunchClip;
                break;

            case PKeys.NPUNCH:
                if (normalPunchClips.Length > 0)
                {
                    int rand = UnityEngine.Random.Range(0, normalPunchClips.Length);
                    usedClip = normalPunchClips[rand];
                }
                break;

            case PKeys.HPunch:
                if (heavyPunchClips.Length > 0)
                {
                    int rand = UnityEngine.Random.Range(0, heavyPunchClips.Length);
                    usedClip = heavyPunchClips[rand];
                }
                break;
            case PKeys.DEATH:
                if (deathSFXClips.Length > 0)
                {
                    int rand = UnityEngine.Random.Range(0, deathSFXClips.Length);
                    usedClip = deathSFXClips[rand];
                }
                break;

        }

        audioSource.PlayOneShot(usedClip, sfxVolume);
    }
    public void PlayInterfaceEffect(string effect)
    {
        AudioClip usedClip = clickedClip;
        
        // randomize pitch
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);

        switch (effect)
        {
            case PKeys.BUTTON:
                usedClip = clickedClip;
                break;
            case PKeys.SLIDER:
                usedClip = sliderClip;
                break;
        }

        audioSource.PlayOneShot(usedClip, menuFXVolume);
    }

    public void SetMuteSetting(bool isMuted)
    {
        IsMuted = isMuted;
        audioSource.mute = isMuted;
    }
    public bool GetMuteSetting()
    {
        return audioSource.mute;
    }
}
public enum SFXType { TileSelected, TileDeselected, SuccessfulCheck, FailedCheck }
public enum InterfaceFXType { Click }
