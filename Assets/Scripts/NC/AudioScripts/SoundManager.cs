using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }
    [SerializeField] private PlayerRefactor player;
    [SerializeField] private AudioClip singleJump;
    [SerializeField] private AudioClip doubleJump;
    [SerializeField] private AudioClip landing;
    [SerializeField] private AudioClip walking;
    private AudioSource _audioSource;
    private float walkingClipTimer = 0f;
    private float walkingClipTimerMax = 0.19f;
    private float volume = 0.5f;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 0.5f);
        _audioSource.volume = volume;
    }

    private void Start()
    {
        if(player != null){
            player.SingleJumpActivated += Player_SingleJumpActivated;
            player.DoubleJumpActivated += Player_DoubleJumpActivated;
            player.LandingActivated += Player_LandingActivated;
        }
    }

    private void Update()
    {
        if(player != null){
            if (player.isWalking())
            {
                if (walkingClipTimer <= 0f)
                {
                    walkingClipTimer = walkingClipTimerMax;
                    playWalkingClip();
                }
            }
            if (walkingClipTimer > 0f)
            {
                walkingClipTimer -= Time.deltaTime;
            }
        }
    }

    private void playWalkingClip()
    {
            _audioSource.PlayOneShot(walking);
    }

    private void Player_LandingActivated(object sender, EventArgs e)
    {
        _audioSource.PlayOneShot(landing);
    }

    private void Player_DoubleJumpActivated(object sender, EventArgs e)
    {
        _audioSource.PlayOneShot(doubleJump);
    }

    private void Player_SingleJumpActivated(object sender, EventArgs e)
    {
        _audioSource.PlayOneShot(singleJump);
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1f)
        {
            volume = 0f;
        }
        _audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
