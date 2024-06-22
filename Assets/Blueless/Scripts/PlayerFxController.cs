using System;

namespace Blueless
{
  using Quantum;
  using UnityEngine;

  /// <summary>
  /// This Behavior handles events related to a player character that should have visual fx, such as landing, double jump and jump
  /// </summary>
  public class PlayerFxController : QuantumEntityViewComponent
  {
    public ParticleSystem LandingParticle;
    public ParticleSystem DoubleJumpParticle;
    public ParticleSystem ExplosionParticle;
    public ParticleSystem JumpParticle;
    public ParticleSystem RespawnParticle;

    public GameObject BodyParent;
    public GameObject UIParent;

    public Renderer[] RobotMeshes;
    public Renderer[] RobotUi;

    public override void OnActivate(Frame frame)
    {
      QuantumEvent.Subscribe<EventOnRobotDoubleJump>(this, OnDoubleJump);
      QuantumEvent.Subscribe<EventOnRobotGrounded>(this, OnGrounded);
      QuantumEvent.Subscribe<EventOnRobotDeath>(this, OnDeath);
      QuantumEvent.Subscribe<EventOnRobotRespawn>(this, OnRespawn);
      QuantumEvent.Subscribe<EventOnRobotJump>(this, OnJump);

      RobotMeshes = BodyParent.GetComponentsInChildren<Renderer>(true);
      RobotUi = UIParent.GetComponentsInChildren<Renderer>();
    }

    private void OnDestroy()
    {
       QuantumEvent.UnsubscribeListener(this);
    }


    private void OnDoubleJump(EventOnRobotDoubleJump eventData)
    {
      if (EntityRef.Equals(eventData.Robot))
      {
        DoubleJumpParticle.Play();
      }
    }

    private void OnJump(EventOnRobotJump eventData)
    {
      if (EntityRef.Equals(eventData.Robot))
      {
        JumpParticle.Play();
      }
    }

    private void OnGrounded(EventOnRobotGrounded eventData)
    {
      if (EntityRef.Equals(eventData.Robot))
      {
        LandingParticle.Play();
      }
    }

    private void OnDeath(EventOnRobotDeath eventData)
    {
      if (!EntityRef.Equals(eventData.Robot))
        return;

      ExplosionParticle.Play();
      foreach (Renderer r in RobotMeshes)
        r.enabled = false;
      foreach (Renderer r in RobotUi)
        r.enabled = false;
    }

    private void OnRespawn(EventOnRobotRespawn eventData)
    {
      if (!EntityRef.Equals(eventData.Robot))
        return;

      RespawnParticle.Play();
      foreach (Renderer r in RobotMeshes)
        r.enabled = true;
      foreach (Renderer r in RobotUi)
        r.enabled = true;
    }
  }
}