namespace Blueless
{
  using System.Collections.Generic;
  using Quantum;
  using UnityEngine;

  /// <summary>
  /// This Behavior handles events related to player actions that require audio feedback, such as Jump, Double Jumping, Death and Respawn
  /// 
  /// Uses the default Unity Audio API for simplicity
  /// </summary>
  public unsafe class PlayerAudioController : QuantumEntityViewComponent
  {
    [Header("References")] public AudioListener AudioListener;
    public AudioSource AudioSourcePrefab;

    [Header("Configurations")] public int MaxAudioSources = 8;
    public Transform AudioSourceParent;

    [Header("Audio Refs")] public AudioConfiguration JumpAudioClip;
    public AudioConfiguration DoubleJumpAudioClip;
    public AudioConfiguration LandingAudioClip;
    public AudioConfiguration DeathClip;
    public AudioConfiguration RespawnClip;
    public AudioConfiguration FootstepsClip;

    private readonly Stack<AudioSource> _freeAudioSources = new Stack<AudioSource>();
    private List<AudioSource> _audioSourcesInUse = new List<AudioSource>();

    public override void OnActivate(Frame frame)
    {
      base.OnActivate(frame);
      for (int i = 0; i < MaxAudioSources; i++)
      {
        var audioSource = Instantiate(AudioSourcePrefab, AudioSourceParent);
        audioSource.transform.localPosition = Vector3.zero;

        _freeAudioSources.Push(audioSource);
      }

      // Register for relevant events		
      QuantumEvent.Subscribe<EventOnRobotJump>(this, OnJump);
      QuantumEvent.Subscribe<EventOnRobotDoubleJump>(this, OnDoubleJump);
      QuantumEvent.Subscribe<EventOnRobotGrounded>(this, OnLanding);
      QuantumEvent.Subscribe<EventOnRobotDeath>(this, OnDeath);
      QuantumEvent.Subscribe<EventOnRobotRespawn>(this, OnRespawn);
      CheckLocalAudioListener();
    }

    private void OnDestroy()
    {
      QuantumEvent.UnsubscribeListener(this);
    }

    private void CheckLocalAudioListener()
    {
      if (VerifiedFrame.Exists(EntityRef) == false)
      {
        return;
      }

      var player = VerifiedFrame.Get<PlayerLink>(EntityRef);
      if (Game.PlayerIsLocal(player.PlayerRef))
      {
        AudioListener.enabled = true;
        var al = Camera.main.GetComponent<AudioListener>();
        Destroy(al);
      }
    }

    void Update()
    {
      for (var i = _audioSourcesInUse.Count - 1; i >= 0; i--)
      {
        var source = _audioSourcesInUse[i];
        if (!source.isPlaying)
        {
          _freeAudioSources.Push(source);
          _audioSourcesInUse.RemoveAt(i);
        }
      }
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
      source.Play();
    }

    private void OnJump(EventOnRobotJump obj)
    {
      if (EntityRef.Equals(obj.Robot))
      {
        PlayAudioClip(JumpAudioClip);
      }
    }

    private void OnDoubleJump(EventOnRobotDoubleJump obj)
    {
      if (EntityRef.Equals(obj.Robot))
      {
        PlayAudioClip(DoubleJumpAudioClip);
      }
    }

    private void OnLanding(EventOnRobotGrounded obj)
    {
      if (EntityRef.Equals(obj.Robot))
        PlayAudioClip(LandingAudioClip);
    }

    private void OnDeath(EventOnRobotDeath obj)
    {
      if (EntityRef.Equals(obj.Robot))
        PlayAudioClip(DeathClip);
    }

    private void OnRespawn(EventOnRobotRespawn eventData)
    {
      if (EntityRef.Equals(eventData.Robot))
      {
        PlayAudioClip(RespawnClip);
      }
    }

    // Triggered via animation events
    public void OnFootStep()
    {
      PlayAudioClip(FootstepsClip);
    }
  }
}