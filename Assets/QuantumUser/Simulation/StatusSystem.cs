namespace Quantum
{
  using Photon.Deterministic;

  /// <summary>
  ///   Manages health and status effects (such as invencibility and death)
  /// </summary>
  public unsafe class StatusSystem : SystemMainThreadFilter<StatusSystem.Filter>, ISignalOnRobotRespawn,
    ISignalOnRobotHit, ISignalOnRobotSkillHit
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public Status* Status;
    }

    void ISignalOnRobotHit.OnRobotHit(Frame frame, EntityRef bullet, EntityRef robot, FP damage)
    {
      EntityRef shooter = frame.Get<BulletFields>(bullet).Source;
      TakeDamage(frame, shooter, robot, damage);
    }


    public void OnRobotRespawn(Frame frame, EntityRef robot)
    {
      Status* status = frame.Unsafe.GetPointer<Status>(robot);
      StatusData statusData = frame.FindAsset<StatusData>(status->StatusData.Id);

      status->IsDead = false;
      status->CurrentHealth = statusData.MaxHealth;
      status->InvincibleTimer = statusData.InvincibleTime;
    }

    void ISignalOnRobotSkillHit.OnRobotSkillHit(Frame frame, EntityRef skillRef, EntityRef robotRef)
    {
      SkillFields skillFields = frame.Get<SkillFields>(skillRef);
      SkillData skillData = frame.FindAsset<SkillData>(skillFields.SkillData.Id);
      EntityRef caster = skillFields.Source;
      TakeDamage(frame, caster, robotRef, skillData.Damage);
    }

    public override void Update(Frame frame, ref Filter filter)
    {
      var status = filter.Status;

      StatusData statusData = frame.FindAsset<StatusData>(status->StatusData.Id);
      status->RegenTimer -= frame.DeltaTime;
      if (status->RegenTimer < 0)
      {
        status->CurrentHealth += frame.DeltaTime * statusData.RegenRate;
        status->CurrentHealth = FPMath.Clamp(status->CurrentHealth, status->CurrentHealth,
          statusData.MaxHealth);
      }

      if (status->InvincibleTimer > FP._0)
      {
        status->InvincibleTimer -= frame.DeltaTime;
      }
    }

    private static void TakeDamage(Frame frame, EntityRef enemy, EntityRef robot, FP damage)
    {
      Status* robotStatus = frame.Unsafe.GetPointer<Status>(robot);

      if (robotStatus->InvincibleTimer > FP._0 || damage < FP._1)
      {
        return;
      }

      StatusData statusData = frame.FindAsset<StatusData>(robotStatus->StatusData.Id);

      robotStatus->RegenTimer = statusData.TimeUntilRegen;
      robotStatus->CurrentHealth -= damage;
      frame.Events.OnRobotTakeDamage(robot, damage, enemy);
      frame.Events.OnRobotBlink(robot);

      if (robotStatus->CurrentHealth <= 0)
      {
        KillRobot(frame, enemy, robot, robotStatus, statusData.RespawnTime);
      }
    }

    private static void KillRobot(Frame frame, EntityRef killer, EntityRef robot, Status* robotStatus, FP respawnTime)
    {
      CharacterController2D* characterController = frame.Unsafe.GetPointer<CharacterController2D>(robot);
      PhysicsCollider2D* collider = frame.Unsafe.GetPointer<PhysicsCollider2D>(robot);

      robotStatus->CurrentHealth = FP._0;
      robotStatus->IsDead = true;
      robotStatus->RespawnTimer = respawnTime;
      characterController->Velocity = FPVector2.Zero;
      collider->IsTrigger = true;

      frame.Signals.OnRobotDeath(robot, killer);
      frame.Events.OnRobotDeath(robot, killer);
    }
  }
}