namespace Quantum
{
  using Photon.Deterministic;

  /// <summary>
  ///   Handles all skill entity interactions
  ///   Things this system handles:
  ///   - Skill Life Cycle
  ///   - Activation timers
  ///   - Skill projectile movement
  ///   - Skill collision checks
  /// </summary>
  public unsafe class SkillSystem : SystemMainThreadFilter<SkillSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public SkillFields* SkillFields;
    }

    public override void Update(Frame frame, ref Filter filter)
    {
      var skill = filter.Entity;
      var skillTransform = filter.Transform;
      var skillFields = filter.SkillFields;

      if (skillFields->TimeToActivate <= FP._0)
      {
        DealAreaDamage(frame, skill, skillTransform);
        frame.Destroy(skill);
      }
      else
      {
        skillFields->TimeToActivate -= frame.DeltaTime;
      }
    }

    private static void DealAreaDamage(Frame frame, EntityRef skill, Transform2D* skillTransform)
    {
      SkillData skillData = frame.FindAsset<SkillData>(frame.Get<SkillFields>(skill).SkillData.Id);
      frame.Events.OnSkillActivated(skillTransform->Position);

      Physics2D.HitCollection hits =
        frame.Physics2D.OverlapShape(*skillTransform, skillData.ShapeConfig.CreateShape(frame));
      for (int i = 0; i < hits.Count; i++)
      {
        EntityRef entity = hits[i].Entity;

        if (entity == skill)
        {
          continue;
        }

        SkillFields skillFields = frame.Get<SkillFields>(skill);
        EntityRef skillSourceEntity = skillFields.Source;

        // Only consider robots for damage
        if (entity == EntityRef.None || frame.Has<Status>(entity) == false)
        {
          continue;
        }

        // Only deal damages to robots not behind walls 
        FPVector2 robotPosition = frame.Get<Transform2D>(entity).Position;
        if (LineOfSightHelper.HasLineOfSight(frame, skillTransform->Position, robotPosition) == false)
        {
          continue;
        }

        //Don't hit the caster robot!
        if (entity == skillSourceEntity)
        {
          continue;
        }

        frame.Signals.OnRobotSkillHit(skill, entity);
        frame.Events.OnSkillHitTarget(skillTransform->Position, skillFields.SkillData.Id.Value, entity);
      }
    }
  }
}