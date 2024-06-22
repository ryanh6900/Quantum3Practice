using System;

namespace Blueless
{
  using UnityEngine;
  using Quantum;

  public sealed unsafe class PlayerAnimatorObserver : QuantumEntityViewComponent<CustomViewContext>
  {
    public Animator Animator;
    public Vector3 Velocity;

    private Vector3 _lastPosition;

    public override void OnActivate(Frame frame)
    {
      QuantumEvent.Subscribe<EventOnRobotDoubleJump>(this, OnDoubleJump);
    }

    private void OnDoubleJump(EventOnRobotDoubleJump eventData)
    {
      if (EntityRef.Equals(eventData.Robot))
      {
        Animator.SetTrigger("DoubleJump");
      }
    }

    public void Update()
    {
      if (ViewContext?.LocalPlayerView != null)
      {
        if (EntityView.EntityRef == ViewContext.LocalPlayerView.EntityRef)
        {
          UpdatePlayerAnimations(true);
        }
      }
    }

    public override void OnUpdateView()
    {
      if (ViewContext?.LocalPlayerView!= null && ViewContext.LocalPlayerView.EntityRef != EntityView.EntityRef )
      {
        UpdatePlayerAnimations(false);
      }
    }

    private void UpdatePlayerAnimations(bool isLocal)
    {
      if (PredictedFrame == null || PredictedFrame.Exists(EntityRef) == false) return;
      
      var robotMovement = PredictedFrame.Get<Movement>(EntityRef);
      bool isFacingRight = isLocal ? ViewContext.LocalPlayerLastDirection.X > 0 : robotMovement.IsFacingRight;
      Animator.SetBool("IsFacingRight", isFacingRight);
      
      var kcc = PredictedFrame.Get<CharacterController2D>(EntityRef);
      Animator.SetBool("IsGrounded", kcc.Grounded);

      Velocity = (transform.position - _lastPosition) / Time.deltaTime;
      _lastPosition = transform.position;
      
      var vel = Velocity.x;
      if (isFacingRight == false)
      {
        vel *= -1;
      }
      Animator.SetFloat("VelocityX", vel);
    }

    private void OnDestroy()
    {
      QuantumEvent.UnsubscribeListener(this);
    }
  }
}