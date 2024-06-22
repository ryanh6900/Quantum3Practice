namespace Blueless
{
  using UnityEngine;
  using Quantum;

  public unsafe class FootstepAudioController : QuantumEntityViewComponent
  {
    public PlayerAudioController AudioController;
    public float StepsDelay;
    public float VelocityThreshold = 0.5f;

    private float _timer;

    public override void OnUpdateView()
    {
      if (PredictedFrame.TryGet<CharacterController2D>(EntityRef, out var kcc))
      {
        if (kcc.Grounded && Mathf.Abs(kcc.Velocity.X.AsFloat) > VelocityThreshold)
        {
          _timer -= Time.deltaTime;
          if (_timer <= 0)
          {
            PlayFootstep();
          }
        }
      }
    }

    private void PlayFootstep()
    {
      _timer = StepsDelay;
      AudioController.OnFootStep();
    }
  }
}