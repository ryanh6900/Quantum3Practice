using System;

namespace Blueless
{
  using Photon.Deterministic;
  using UnityEngine;
  using UnityEngine.InputSystem;
  using Quantum;

  public unsafe class LocalGameplayInput : QuantumSceneViewComponent<CustomViewContext>
  {
    public float AimAssist = 20;
    public float AimSpeed = 2;

    private PlayerInput _playerInput;
    private Vector2 _lastPlayerDirection;

    public override void OnInitialize()
    {
      QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback),
        onlyIfActiveAndEnabled: true);
      _playerInput = GetComponent<PlayerInput>();
    }

    public override void OnUpdateView()
    {
      ViewContext.LocalPlayerLastDirection = GetAimDirection();
    }

    public void PollInput(CallbackPollInput callback)
    {
      Quantum.Input input = new Quantum.Input();

      if (callback.Game.GetLocalPlayers().Count == 0)
      {
        return;
      }

      input.Fire = _playerInput.actions["Fire"].IsPressed();

      //#if UNITY_MOBILE || UNITY_EDITOR -> to test touch UI
#if UNITY_MOBILE
    var aimDirection = _playerInput.actions["Aim"].ReadValue<Vector2>();
    input.Fire = (aimDirection.magnitude >= 0.5f);
#endif

      input.Jump = _playerInput.actions["Jump"].IsPressed();
      input.Movement = (sbyte)(GetMovement() * sbyte.MaxValue).AsInt;
      input.AimDirection = GetAimDirection();
      input.ChangeWeapon = _playerInput.actions["ChangeWeapon"].IsPressed();
      input.CastSkill = _playerInput.actions["CastSkill"].IsPressed();

      callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }

    private FP GetMovement()
    {
      FPVector2 directional = _playerInput.actions["Move"].ReadValue<Vector2>().ToFPVector2();
      return directional.X;
    }

    private FPVector2 GetAimDirection()
    {
      Vector2 direction = Vector2.zero;
      var localPlayerView = ViewContext.LocalPlayerView;
      if (localPlayerView == null)
      {
        return FPVector2.Zero;
      }

      EntityRef robot = localPlayerView.EntityRef;
      Frame frame = localPlayerView.PredictedFrame;

      var isMobile = false;

#if !UNITY_STANDALONE && !UNITY_WEBGL
      isMobile = true;
# endif
      if (frame.TryGet<Transform2D>(robot, out var robotTransform))
      {
        if (isMobile || Gamepad.all.Count != 0)
        {
          Vector2 directional = _playerInput.actions["Aim"].ReadValue<Vector2>();
          var controlDir = new Vector2(directional.x, directional.y);
          if (controlDir.sqrMagnitude > 0.1f)
          {
            direction = controlDir;
          }
          else if (Mathf.Abs(GetMovement().AsFloat) > 0.1f)
          {
            direction = new Vector2(GetMovement().AsFloat, 0);
          }
          else
          {
            direction = _lastPlayerDirection;
          }

          _lastPlayerDirection = direction;

          //AIM ASSIST
          var minorAngle = AimAssist;
          var position = frame.Get<Transform2D>(localPlayerView.EntityRef).Position;

          var targetDirection = position - robotTransform.Position;
          if (Vector2.Angle(direction, targetDirection.ToUnityVector2()) <= minorAngle)
          {
            direction = Vector2.Lerp(direction, targetDirection.ToUnityVector2(), Time.deltaTime * AimSpeed);
          }
        }
        else
        {
          var localRobotPosition = robotTransform.Position.ToUnityVector3();
          var localRobotScreenPosition = Camera.main.WorldToScreenPoint(localRobotPosition);
          var mousePos = _playerInput.actions["MousePosition"].ReadValue<Vector2>();
          direction = mousePos - new Vector2(localRobotScreenPosition.x, localRobotScreenPosition.y);
        }

        return new FPVector2(FP.FromFloat_UNSAFE(direction.x), FP.FromFloat_UNSAFE(direction.y));
      }
      return FPVector2.Zero;
    }
  }
}