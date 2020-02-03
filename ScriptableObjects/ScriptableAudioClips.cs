using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClips", menuName = "ScriptableObjects/AudioClips", order = 9)]
[Serializable]

/**
 * Storing All AudioClips References... in An Asset File
 */
public class ScriptableAudioClips : ScriptableObject
{
    [Header("Music Sounds")]
    //Music Sounds
    public List<AudioClip> _GameLevelsSound;
    [Header("Item Shop Music Sounds")]
    public AudioClip ItemShopSound;

    [Header("GameOver Sound")]
    public AudioClip _GameOverSound;
    [Header("Finish Level Sound")]
    public AudioClip _FinishedLevelSound;
    [Header("StageCompleteSounds")]
    public AudioClip _HelicopterStageCompleteSound;
    [Header("OnFinalBossKilledMusic")]
    public AudioClip OnFinalBossKilledMusic;

    [Space]
    [Header("PlayerSounds")]

    [Header("SFX Sounds")]

    public AudioClip _LaraJumpSound;
    public AudioClip _LaraLandingSound;
    public List<AudioClip> _LaraFootStepSound;
    public List<AudioClip> _LaraAttackSounds;
    public AudioClip _LaraShootSound;
    public List<AudioClip> _LaraHurtSound;
    public AudioClip _LaraDeathSound;

    //SFX Sounds
    [Header("Button Clicked Sound")]
    public AudioClip ButtonSound;

    [Header("TurrentLauncher")]
    //Turret Launcher Sounds
    public AudioClip TurretLaunchSound;
    public AudioClip TurretDestoryedSound;
    [Header("CannonFlare Sounds")]
    public AudioClip FlareSound;

    [Header("EnemySounds")]
    public AudioClip EnemyWithPistolBulletSound;
    public AudioClip EnemyWithAutoGunBulletSound;
    public AudioClip EnemyWithAutoGunReloadSound;
    public List<AudioClip> EnemyHurtSound;
    public AudioClip EnemyDeathSound;

    [Header("Falling Stone Sounds")]
    public AudioClip FallingStoneCrumbleSound;

    [Header("Teleporter Sounds")]
    public AudioClip TeleporterSound;

    [Header("PushableBox Sounds")]
    public AudioClip PushableBoxSound;

    [Header("GotKeySounds")]
    public AudioClip GotKeySound;

    [Header("GotCoinSounds")]
    public AudioClip GotCashSound;
    public AudioClip BoughtCashSound;

    [Header("Got Shield Sound")]
    public AudioClip GotShieldSound;

    [Header("Got Live Sound")]
    public AudioClip GotLiveSound;

    [Header("StoneDoorSounds")]
    public AudioClip StoneDoorOpeningSound;
    public AudioClip StoneDoorClosingSound;

    [Header("GotAmmoBoss")]
    public AudioClip GotAmmoBossSound;

    [Header("HelicopterSounds")]
    public AudioClip HelicopterFlyingSound;
    public AudioClip HelicopterBeingDestoryedSound;
    public AudioClip HelicopterBulletShootingSound;
    public AudioClip HelicopterMissileLaunchSound;
    public AudioClip HelicopterMissileDestoryedSound;

    [Header("Boulder Sounds")]
    public AudioClip BoulderRollingSound;

    [Header("Moving Surface Sounds")]
    public List<AudioClip> MovingSurfaceSounds;

    [Header("Final Boss Sounds")]
    public AudioClip FinalBossHurtSound;
    public AudioClip FinalBossDeathSound;
}