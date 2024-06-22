namespace Quantum
{
  using Photon.Deterministic;
  using Quantum.Core;

  /// <summary>
  ///   Handles movement and input for all players
  ///   Things this system handles:
  ///   - Robot Movement
  ///   - Robot Jump & Double Jump
  /// </summary>
  public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>, ISignalOnGameEnded,
    IKCCCallbacks2D
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public PlayerLink* PlayerLink;
      public Status* Status;
      public Movement* RobotMovement;
      public CharacterController2D* KCC;
    }


    public override void Update(Frame frame, ref Filter filter)
    {
      var robot = filter.Entity;
      var transform = filter.Transform;
      var player = filter.PlayerLink;
      var status = filter.Status;
      var robotMovement = filter.RobotMovement;
      var kcc = filter.KCC;

      if (status->IsDead == true)
      {
        return;
      }

      Input* input = frame.GetPlayerInput(player->PlayerRef);

      FP axisHorizontal = input->Movement;
      axisHorizontal /= sbyte.MaxValue;

      FP moveScale = FP._1;
      FPVector2 newPosition = Move(frame, ref filter, new FPVector2(moveScale * axisHorizontal, 0), this);
      transform->Position += newPosition;
      MovementData movementData = frame.FindAsset<MovementData>(robotMovement->MovementData.Id);

      if (robotMovement->PrevGrounded == false && robotMovement->VirtualGrounded)
      {
        robotMovement->JumpDelayTimer = movementData.JumpDelay;
        robotMovement->CanDoubleJump = true;
        frame.Events.OnRobotGrounded(robot);
      }
      else
      {
        robotMovement->JumpDelayTimer -= frame.DeltaTime;
      }

      robotMovement->PrevGrounded = robotMovement->VirtualGrounded;

      if (input->Jump.WasPressed)
      {
        if (kcc->Grounded || robotMovement->JumpDelayTimer > 0)
        {
          kcc->Jump(frame);
          frame.Events.OnRobotJump(robot);
        }
        else if (robotMovement->CanDoubleJump)
        {
          kcc->Jump(frame, true, movementData.SecondJumpImpulse);
          robotMovement->CanDoubleJump = false;

          frame.Events.OnRobotDoubleJump(robot);
        }
      }

      UpdateIsFacingRight(frame, input, robot);
    }

    private void UpdateIsFacingRight(Frame frame, Input* input, EntityRef robot)
    {
      Movement* robotMovement = frame.Unsafe.GetPointer<Movement>(robot);
      robotMovement->IsFacingRight = input->AimDirection.X > 0;
    }

    public void OnGameEnded(Frame frame, GameController* gameController)
    {
      foreach (var (robot, kcc) in frame.Unsafe.GetComponentBlockIterator<CharacterController2D>())
      {
        kcc->Velocity = FPVector2.Zero;
      }

      frame.SystemDisable<MovementSystem>();
    }

    private FPVector2 Move(Frame frame, ref Filter filter, FPVector2 velocity, IKCCCallbacks2D callback = null)
    {
      var robot = filter.Entity;
      var transform = filter.Transform;
      var robotMovement = filter.RobotMovement;
      var kcc = filter.KCC;

      int layer = frame.Layers.GetLayerMask("Environment");
      CharacterController2DMovement movementPack =
        CharacterController2D.ComputeRawMovement(frame, robot, transform, kcc, velocity, callback, layer);
      CheckVisualGrounded(frame, robotMovement, movementPack, robot);

      CharacterController2DConfig kccConfig = frame.FindAsset<CharacterController2DConfig>(kcc->Config.Id);
      ComputeRawSteer(kcc, ref movementPack, frame.DeltaTime, kccConfig);
      FPVector2 movement = kcc->Velocity * frame.DeltaTime;

      if (movementPack.Penetration > FP.EN3)
      {
        if (movementPack.Penetration > kccConfig.MaxPenetration)
        {
          movement += movementPack.Correction;
        }
        else
        {
          movement += movementPack.Correction * frame.DeltaTime * kccConfig.Acceleration;
        }
      }

      return movement;
    }

    private void CheckVisualGrounded(Frame frame, Movement* robotMovement, CharacterController2DMovement movementPack,
      EntityRef robot)
    {
      if (movementPack.Grounded)
      {
        robotMovement->GroundedFramesCount += 1;
      }
      else
      {
        robotMovement->UngroundedFramesCount += 1;
      }

      if (movementPack.Grounded == false && robotMovement->UngroundedFramesCount >= 10)
      {
        robotMovement->VirtualGrounded = false;
        robotMovement->GroundedFramesCount = 0;
        robotMovement->UngroundedFramesCount = 0;
      }

      if (movementPack.Grounded && robotMovement->GroundedFramesCount >= 1)
      {
        robotMovement->VirtualGrounded = true;
        robotMovement->GroundedFramesCount = 0;
        robotMovement->UngroundedFramesCount = 0;
      }
    }

    static void ComputeRawSteer(CharacterController2D* kcc, ref CharacterController2DMovement movementPack,
      FP deltaTime, CharacterController2DConfig config)
    {
      kcc->Grounded = movementPack.Grounded;

      FP maxYSpeed = FP._100;
      FP minYSpeed = -FP._100;
      switch (movementPack.Type)
      {
        case CharacterMovementType.FreeFall:
          kcc->Velocity.Y -= config.Gravity.Magnitude * deltaTime;
          if (config.AirControl == false || movementPack.Tangent == default(FPVector2))
          {
            kcc->Velocity.X = FPMath.Lerp(kcc->Velocity.X, FP._0, deltaTime * config.Braking);
          }
          else
          {
            kcc->Velocity.X += movementPack.Tangent.X * config.Acceleration * deltaTime;
          }

          break;
        case CharacterMovementType.Horizontal:
          kcc->Velocity.X += movementPack.Tangent.X * config.Acceleration * deltaTime;
          kcc->Velocity.Y += movementPack.Tangent.Y * config.Acceleration * deltaTime;
          if (kcc->Velocity.Y <= 0)
          {
            if (kcc->Velocity.SqrMagnitude > kcc->MaxSpeed * kcc->MaxSpeed)
            {
              kcc->Velocity = kcc->Velocity.Normalized * kcc->MaxSpeed;
            }
          }

          break;
        case CharacterMovementType.SlopeFall:
          kcc->Velocity += movementPack.SlopeTangent * config.Acceleration * deltaTime;
          minYSpeed = -config.MaxSlopeSpeed;
          break;
        case CharacterMovementType.None:
          if (kcc->Velocity != default(FPVector2))
          {
            if (kcc->Velocity.Y <= 0)
            {
              kcc->Velocity = FPVector2.Lerp(kcc->Velocity, default(FPVector2), deltaTime * config.Braking);
            }

            if (kcc->Velocity.SqrMagnitude < FP._0_10)
            {
              kcc->Velocity = default(FPVector2);
            }
          }

          minYSpeed = FP._0;
          break;
      }

      if (movementPack.Type != CharacterMovementType.Horizontal)
      {
        kcc->Velocity.Y = FPMath.Clamp(kcc->Velocity.Y, minYSpeed, maxYSpeed);
        kcc->Velocity.X = FPMath.Clamp(kcc->Velocity.X, -kcc->MaxSpeed, kcc->MaxSpeed);
      }
    }

    public bool OnCharacterCollision2D(FrameBase frame, EntityRef character, Physics2D.Hit hit)
    {
      // stops jumps when hitting ceilings
      CharacterController2D* kcc = frame.Unsafe.GetPointer<CharacterController2D>(character);
      if (hit.Normal.Y.RawValue < 0 && kcc->Velocity.Y.RawValue > 0)
      {
        kcc->Velocity.Y.RawValue = 0;
        kcc->Grounded = false;
      }

      return true;
    }

    public void OnCharacterTrigger2D(FrameBase f, EntityRef character, Physics2D.Hit hit)
    {
    }
  }
}