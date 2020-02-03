using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/**
 * AudioManager For Game, Handling Sound Effects Menu, SFX,Music Etc..
 */
public class AudioManager : MonoBehaviour
{
    public ScriptableAudioClips _AudioClips;
    public AudioData _AudioData;//Audio Volumes Data

    public AudioMixer _AudioMixer;

    //Music Sounds
    public AudioSource _MusicSource;// OnLoopSource

    //SFX Sounds
    public AudioSource _PlayerSource;
    public AudioSource _InGameSounds;

    public AudioSource _MenuSFXSounds;

    private AudioSource ItemShopMusicSource;
    private AudioSource ItemShopSFXSource;

    public UnityEvent OnCreatingItemShopAudioSource;

    private void Awake()//After the Scene is Loaded invoke the method OnSceneLoaded
    {
        _MenuSFXSounds.ignoreListenerPause = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnCreatingItemShopAudioSource?.Invoke();
    }
    private void OnDestroy()//On Loading Another Scene,This Method Being Invoked , To Prevent Changing Volumes while Loading The Actual Scene,Results an Error
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene WhichScene, LoadSceneMode Mode)
    {
        ChangeSFXVolume();
        ChangeMusicVolume();
        PlayLevelSound(WhichScene.buildIndex);
    }
    private void PlayLevelSound(int WhichSceneIndex)
    {
        _MusicSource.clip = _AudioClips._GameLevelsSound[WhichSceneIndex];
        _MusicSource.Play();
    }
    public void OnSpeakerModeChanged(AudioSpeakerMode SpeakerMode)
    {
        AudioConfiguration _AudioConfiguration = AudioSettings.GetConfiguration();
        _AudioConfiguration.speakerMode = SpeakerMode;
        AudioSettings.Reset(_AudioConfiguration);
        _MusicSource.Play();
    }

    public void PlaySwitchToggle(AudioSource SwitchToggleSource)
    {
        SwitchToggleSource.volume = _InGameSounds.volume;
        SwitchToggleSource.Play();
    }

    public void PauseAudioOnPauseGame()
    {
        AudioListener.pause = true;
    }
    public void UnPauseAudioOnResumeGame()
    {
        AudioListener.pause = false;
    }
    public void ClickSound()//OnClickSoundEvent
    {
        _MenuSFXSounds.clip = _AudioClips.ButtonSound;
        _MenuSFXSounds.Play();
    }
    public void ChangeMasterVolume()
    {
        ChangeSFXVolume();
        ChangeMusicVolume();
    }
    public void ChangeMusicVolume()
    {
        _MusicSource.volume = _AudioData.MusicAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
    }
    public void ChangeSFXVolume()
    {
        _PlayerSource.volume = _AudioData.SFXAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
        _InGameSounds.volume = _AudioData.SFXAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
        _MenuSFXSounds.volume = _AudioData.SFXAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
    }

    #region GameOverSound
    public void PlayGameOverSoundOnDying()
    {
        _MenuSFXSounds.PlayOneShot(_AudioClips._GameOverSound);
    }
    #endregion

    #region FinishedLevelSound
    public void PlayFinishedLevelSound()
    {
        _MenuSFXSounds.clip = _AudioClips._FinishedLevelSound;
        _MenuSFXSounds.Play();
    }
    #endregion

    #region StageCompleteSounds
    public void PlayHelicopterStageCompleteMusicSound()
    {
        _MusicSource.Stop();
        _MenuSFXSounds.clip = _AudioClips._HelicopterStageCompleteSound;
        _MenuSFXSounds.Play();
    }
    #endregion

    #region ItemShopMusicSounds
    //OnEvent
    public void CreateItemShopAudioSource()
    {
        GameObject ItemShopSource = new GameObject("ItemShopMusicSource");
        GameObject _ItemShopSFXSource = new GameObject("ItemShopSFXSource");
        ItemShopSource.AddComponent<AudioSource>();
        _ItemShopSFXSource.AddComponent<AudioSource>();
        _ItemShopSFXSource.transform.parent = ItemShopSource.transform;

        ItemShopMusicSource = ItemShopSource.GetComponent<AudioSource>();
        ItemShopMusicSource.playOnAwake = false;
        ItemShopMusicSource.volume = _AudioData.MusicAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
        ItemShopMusicSource.clip = _AudioClips.ItemShopSound;
        ItemShopMusicSource.outputAudioMixerGroup = _AudioMixer.FindMatchingGroups("MenuSFXSounds")[0];
        ItemShopMusicSource.ignoreListenerPause = true;

        ItemShopSFXSource = _ItemShopSFXSource.GetComponent<AudioSource>();
        ItemShopSFXSource.playOnAwake = false;
        ItemShopSFXSource.ignoreListenerPause = true;
        ItemShopSFXSource.volume = _MenuSFXSounds.volume;
        ItemShopSFXSource.outputAudioMixerGroup = _AudioMixer.FindMatchingGroups("MenuSFXSounds")[0];
    }
    public void PlayItemShopSound()
    {
        _MenuSFXSounds.Pause();
        _MusicSource.Pause();
        ItemShopMusicSource.Play();
    }
    public void StopPlayingItemShopSound()
    {
        ItemShopMusicSource.Stop();
        _MusicSource.UnPause();
        _MenuSFXSounds.UnPause();
    }
    public void PlayItemShopClickSound()
    {
        ItemShopSFXSource.clip = _AudioClips.ButtonSound;
        ItemShopSFXSource.Play();
    }
    #endregion

    #region PlayerSounds
    public void PlayJumpSound()
    {
        _PlayerSource.PlayOneShot(_AudioClips._LaraJumpSound);
    }
    public void PlayMovingSound()
    {
        _PlayerSource.clip = _AudioClips._LaraFootStepSound[0];
        _PlayerSource.Play();
    }
    public void PlayOnLandingSound()
    {
        _PlayerSource.PlayOneShot(_AudioClips._LaraLandingSound);
    }
    public void PlayShootingSound()
    {
        _PlayerSource.PlayOneShot(_AudioClips._LaraShootSound);
    }
    public void PlayAttackSound()
    {
        _PlayerSource.PlayOneShot(_AudioClips._LaraAttackSounds[UnityEngine.Random.Range(0, _AudioClips._LaraAttackSounds.Count)]);
    }
    public void PlayHurtSound()
    {
        _PlayerSource.PlayOneShot(_AudioClips._LaraHurtSound[UnityEngine.Random.Range(0, _AudioClips._LaraHurtSound.Count)]);
    }
    public void PlayDeathSound()
    {
        _PlayerSource.clip = _AudioClips._LaraDeathSound;
        _PlayerSource.Play();
    }
    #endregion

    #region SFXSounds

    #region TurretSound
    public void PlayTurretLaunchSound(AudioSource TurretSource)
    {
        TurretSource.clip = _AudioClips.TurretLaunchSound;
        TurretSource.volume = _InGameSounds.volume;
        TurretSource.Play();
    }

    public void PlayTurrentDestroyed(AudioSource TurretSource)
    {
        TurretSource.clip = _AudioClips.TurretDestoryedSound;
        TurretSource.volume = _InGameSounds.volume;
        TurretSource.Play();
    }
    #endregion

    #region CannonFlareSound
    public void PlayFlareSound(AudioSource FlareSound)
    {
        FlareSound.clip = _AudioClips.FlareSound;
        FlareSound.volume = _InGameSounds.volume;
        FlareSound.Play();
    }
    #endregion

    #region EnemySounds
    public void PlayEnemyWithPistolBulletSound(AudioSource EnemySoundSource)
    {
        EnemySoundSource.clip = _AudioClips.EnemyWithPistolBulletSound;
        EnemySoundSource.volume = _InGameSounds.volume;
        EnemySoundSource.Play();
    }
    public void PlayEnemyWithAutoGunBulletSound(AudioSource EnemySoundSource)
    {
        EnemySoundSource.clip = _AudioClips.EnemyWithAutoGunBulletSound;
        EnemySoundSource.volume = _InGameSounds.volume;
        EnemySoundSource.Play();
    }
    public void PlayEnemyWithAutoGunReloadSound(AudioSource EnemySoundSource)
    {
        EnemySoundSource.PlayOneShot(_AudioClips.EnemyWithAutoGunReloadSound);
    }
    public void PlayEnemyHurtSound(AudioSource EnemySoundSource)
    {
        EnemySoundSource.PlayOneShot(_AudioClips.EnemyHurtSound[UnityEngine.Random.Range(0, _AudioClips.EnemyHurtSound.Count)]);
    }
    public void PlayEnemyDeathSound(AudioSource EnemySoundSource)
    {
        EnemySoundSource.clip = _AudioClips.EnemyDeathSound;
        EnemySoundSource.volume = _InGameSounds.volume;
        EnemySoundSource.Play();
    }
    #endregion

    #region FallingStoneSounds
    public void PlayCrumbleSound(AudioSource CrumbleSound)
    {
        CrumbleSound.clip = _AudioClips.FallingStoneCrumbleSound;
        CrumbleSound.volume = _InGameSounds.volume;
        CrumbleSound.Play();
    }
    #endregion

    #region TeleporterSounds
    public void PlayTeleporterSound()
    {
        _InGameSounds.PlayOneShot(_AudioClips.TeleporterSound);
    }
    public void StopTeleporterSound()
    {
        _InGameSounds.Stop();
    }
    #endregion

    #region PushableBoxSounds
    public void PlayPushableBoxSound(AudioSource PushableBoxSource)
    {
        PushableBoxSource.clip = _AudioClips.PushableBoxSound;
        PushableBoxSource.volume = _InGameSounds.volume;
        PushableBoxSource.Play();
    }
    public void StopPushableBoxSound(AudioSource PushableBoxSource)
    {
        PushableBoxSource.Stop();
    }
    #endregion

    #region HavingKeySound
    public void PlayGotKeySound()
    {
        _InGameSounds.PlayOneShot(_AudioClips.GotKeySound);
    }
    #endregion

    #region CashSounds
    public void PlayGotCashSound()
    {
        _InGameSounds.PlayOneShot(_AudioClips.GotCashSound);
    }
    public void PlayBoughtCashSound()
    {
        ItemShopSFXSource.clip = _AudioClips.BoughtCashSound;
        ItemShopSFXSource.Play();
    }
    #endregion

    #region HavingShieldSound
    public void PlayGotShieldSound()
    {
        ItemShopSFXSource.clip = _AudioClips.GotShieldSound;
        ItemShopSFXSource.Play();
    }

    #endregion

    #region GotLiveSound
    public void PlayGotLiveSound()
    {
        ItemShopSFXSource.clip = _AudioClips.GotLiveSound;
        ItemShopSFXSource.Play();
    }
    #endregion

    #region StoneDoorSounds
    public void PlayStoneDoorOpen(AudioSource StoneDoorSource)
    {
        StoneDoorSource.clip = _AudioClips.StoneDoorOpeningSound;
        StoneDoorSource.volume = _InGameSounds.volume;
        StoneDoorSource.Play();
    }
    public void PlayStoneDoorClosing(AudioSource StoneDoorSource)
    {
        StoneDoorSource.clip = _AudioClips.StoneDoorClosingSound;
        StoneDoorSource.volume = _InGameSounds.volume;
        StoneDoorSource.Play();
    }
    #endregion

    #region GotAmmoBossSounds
    public void PlayGotAmmoBossSound(AudioSource GotAmmoSource)
    {
        GotAmmoSource.clip = _AudioClips.GotAmmoBossSound;
        GotAmmoSource.volume = _InGameSounds.volume;
        GotAmmoSource.Play();
    }
    #endregion

    #region HelicopterSounds
    public void PlayHelicopterFlyingSound()
    {
        _InGameSounds.PlayOneShot(_AudioClips.HelicopterFlyingSound);
    }
    public void PlayHelicopterBeingDestoryedSound()
    {
        StopHelicopterBeingFlying();
        _InGameSounds.PlayOneShot(_AudioClips.HelicopterBeingDestoryedSound, 1.5f);
    }
    private void StopHelicopterBeingFlying()
    {
        _InGameSounds.Stop();
    }
    public void PlayHelicopterShootingSound(AudioSource HelicopterSource)
    {
        HelicopterSource.clip = _AudioClips.HelicopterBulletShootingSound;
        HelicopterSource.volume = _InGameSounds.volume * .4f;
        HelicopterSource.Play();
    }
    public void PlayHelicopterMissleLaunchSound(AudioSource HelicopterSource)
    {
        HelicopterSource.clip = _AudioClips.HelicopterMissileLaunchSound;
        HelicopterSource.volume = _InGameSounds.volume * .8f;
        HelicopterSource.Play();
    }

    public void PlayHelicopterMissleDestoryedSound(AudioSource MissileSource)
    {
        MissileSource.clip = _AudioClips.HelicopterMissileDestoryedSound;
        MissileSource.volume = _InGameSounds.volume;
        MissileSource.Play();
    }
    #endregion

    #region BoulderSounds
    public void PlayBoulderRollingSound(AudioSource boulder)
    {
        boulder.clip = _AudioClips.BoulderRollingSound;
        boulder.volume = _AudioData.SFXAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
        boulder.Play();
    }
    public void StopBoulderRollingSound(AudioSource boulder)
    {
        boulder.Stop();
    }
    #endregion

    #region MovingSurfaceSound

    public void PlayMovingSurfaceSound(AudioSource MovingSurface, int WhichSound, float DelayTime = 0.0f)
    {
        MovingSurface.clip = _AudioClips.MovingSurfaceSounds[WhichSound];
        MovingSurface.volume = _AudioData.SFXAudioValue / 100f * (_AudioData.MasterAudioValue / 100f);
        MovingSurface.PlayDelayed(DelayTime);
    }
    public void StopMovingSurfaceSound(AudioSource MovingSurface)
    {
        MovingSurface.Stop();
    }
    #endregion

    #region FinalBossSounds
    public void PlayFinalBossHurtSound()
    {
        _InGameSounds.PlayOneShot(_AudioClips.FinalBossHurtSound);
    }
    public void PlayFinalBossDeathSound()
    {
        _InGameSounds.PlayOneShot(_AudioClips.FinalBossDeathSound);
    }
    public void OnFinalBossKilled()
    {
        _MusicSource.Stop();
        _MusicSource.clip = _AudioClips.OnFinalBossKilledMusic;
        _MusicSource.Play();
    }
    #endregion

    #endregion
}