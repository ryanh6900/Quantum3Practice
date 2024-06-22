namespace Blueless
{
  using System.Collections.Generic;
  using Quantum;
  using UnityEngine;

  /// <summary>
  /// This Behavior handles events not related to player actions that require audio feedback, such as shooting and explosions
  /// Uses the default Unity Audio API for simplicity
  /// </summary>
  public class SfxController : QuantumSceneViewComponent<CustomViewContext>
  {
    [Header("References")] public AudioSource AudioSourcePrefab;

    [Header("Configurations")] public int MaxAudioSources = 16;
    public Transform AudioSourceDefaultParent;

    [Header("Audios")] public AudioConfiguration PlayerHitAudio;
    public AudioConfiguration PlayerKillAudio;
    public AudioConfiguration PlayerDamageTakenAudio;
    public AudioConfiguration SkillCastingAudio;
    public AudioConfiguration SkillActivationAudio;
    public QuantumEntityViewUpdater ViewUpdater;

    private readonly Stack<AudioSource> _freeAudioSources = new Stack<AudioSource>();
    private List<AudioSource> _audioSourcesInUse = new List<AudioSource>();


    private void Start()
    {
      for (int i = 0; i < MaxAudioSources; i++)
      {
        var audioSource = Instantiate(AudioSourcePrefab, AudioSourceDefaultParent);
        audioSource.transform.localPosition = Vector3.zero;

        _freeAudioSources.Push(audioSource);
      }

      RegisterCallbacks();
    }

    private void Update()
    {
      for (var i = _audioSourcesInUse.Count - 1; i >= 0; i--)
      {
        var source = _audioSourcesInUse[i];
        if (!source.isPlaying)
        {
          _freeAudioSources.Push(source);
          _audioSourcesInUse.RemoveAt(i);
          source.transform.SetParent(AudioSourceDefaultParent);
          source.transform.position = Vector3.zero;
        }
      }
    }

    private void OnDestroy()
    {
      UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
      QuantumEvent.Subscribe<EventOnRobotTakeDamage>(this, OnRobotDamage);
      QuantumEvent.Subscribe<EventOnWeaponShoot>(this, OnWeaponShot);
      QuantumEvent.Subscribe<EventOnBulletDestroyed>(this, OnBulletDestroyed);
      QuantumEvent.Subscribe<EventOnSkillCasted>(this, OnSkillCasted);
      QuantumEvent.Subscribe<EventOnSkillActivated>(this, OnSkillActivated);
      QuantumEvent.Subscribe<EventOnRobotDeath>(this, OnRobotDeath);
    }

    private void UnregisterCallbacks()
    {
      QuantumEvent.UnsubscribeListener(this);
    }

    AudioSource GetAvailableAudioSource()
    {
      if (_freeAudioSources.Count > 0)
      {
        var source = _freeAudioSources.Pop();
        _audioSourcesInUse.Add(source);
        return source;
      }
      else
      {
        var source = _audioSourcesInUse[0];
        _audioSourcesInUse.RemoveAt(0);
        _audioSourcesInUse.Add(source);
        return source;
      }
    }

    void PlayAudioClip(AudioConfiguration audioConfig)
    {
      var source = GetAvailableAudioSource();
      audioConfig.AssignToAudioSource(source);

      source.transform.position = Vector3.zero;
      source.Play();
    }

    void PlayAudioClip(AudioConfiguration audioConfig, Transform parent)
    {
      var source = GetAvailableAudioSource();
      audioConfig.AssignToAudioSource(source);

      source.transform.SetParent(parent);
      source.transform.localPosition = Vector3.zero;
      source.Play();
    }

    private void PlayAudioClip(AudioConfiguration audioConfig, Vector3 position)
    {
      var source = GetAvailableAudioSource();
      audioConfig.AssignToAudioSource(source);

      source.transform.position = position;
      source.Play();
    }

    private unsafe void OnRobotDamage(EventOnRobotTakeDamage eventData)
    {
      Frame frame = eventData.Game.Frames.Predicted;
      var targetRobotTransform = frame.Get<Transform2D>(eventData.Robot);
      var audioConfig = eventData.Robot == ViewContext.LocalPlayerView.EntityRef
        ? PlayerDamageTakenAudio
        : PlayerHitAudio;
      PlayAudioClip(audioConfig, targetRobotTransform.Position.ToUnityVector3());
    }

    private unsafe void OnWeaponShot(EventOnWeaponShoot eventData)
    {
      Frame frame = eventData.Game.Frames.Verified;
      var weaponInventory = frame.Get<WeaponInventory>(eventData.Robot);
      var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
      var weaponData = frame.FindAsset<WeaponData>(weapon.WeaponData.Id);
    
      var robotView = ViewUpdater.GetView(eventData.Robot);
    
      var robotTransform = frame.Get<Transform2D>(eventData.Robot);
    
      if (robotView != null)
      {
        PlayAudioClip(weaponData.ShootAudioInfo, robotView.transform);
      }
      else
      {
        PlayAudioClip(weaponData.ShootAudioInfo, robotTransform.Position.ToUnityVector3());
      }
    }

    private void OnBulletDestroyed(EventOnBulletDestroyed eventData)
    {
      var asset = QuantumUnityDB.GetGlobalAsset<BulletData>(eventData.BulletData);
      PlayAudioClip(asset.BulletDestroyAudioInfo, eventData.BulletPosition.ToUnityVector3());
    }

    private unsafe void OnSkillCasted(EventOnSkillCasted eventData)
    {
      Frame frame = eventData.Game.Frames.Predicted;
      if (frame.Exists(eventData.Skill) == false)
      {
        return;
      }

      var skillFields = frame.Get<SkillFields>(eventData.Skill);
      var skillTransform = frame.Get<Transform2D>(eventData.Skill);
      var robotView = ViewUpdater.GetView(skillFields.Source);

      if (robotView != null)
      {
        PlayAudioClip(SkillCastingAudio, robotView.transform);
      }
      else
      {
        PlayAudioClip(SkillCastingAudio, skillTransform.Position.ToUnityVector3());
      }
    }

    private void OnSkillActivated(EventOnSkillActivated eventData)
    {
      PlayAudioClip(SkillActivationAudio, eventData.SkillPosition.ToUnityVector3());
    }

    private unsafe void OnRobotDeath(EventOnRobotDeath eventData)
    {
      QuantumGame game = eventData.Game;
      Frame frame = game.Frames.Predicted;
      var player = frame.Get<PlayerLink>(eventData.Killer);
      if (game.PlayerIsLocal(player.PlayerRef))
      {
        PlayAudioClip(PlayerKillAudio);
      }
    }
  }
}