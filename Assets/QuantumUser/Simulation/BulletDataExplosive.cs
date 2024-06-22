namespace Quantum
{
  using Photon.Deterministic;
  /// <summary>
  ///   Explosive bullet behavior
  ///   Explodes on impact, deals damage to all robots in the ExposionRadius.
  /// </summary>

  [System.Serializable]
  public class BulletDataExplosive : BulletData
  {
    public Shape2DConfig ExplosionShape;
    
    public override unsafe void BulletAction(Frame frame, EntityRef bullet, EntityRef targetRobot)
    {
      Explode(frame, bullet, targetRobot);

      if (targetRobot != EntityRef.None)
      {
        frame.Signals.OnRobotHit(bullet, targetRobot, Damage);
      }

      BulletFields bulletFields = frame.Get<BulletFields>(bullet);
      FPVector2 bulletPosition = frame.Get<Transform2D>(bullet).Position;

      frame.Events.OnBulletDestroyed(bullet.GetHashCode(), bulletFields.Source, bulletPosition, bulletFields.Direction, Guid);
      frame.Destroy(bullet);
    }

    private unsafe void Explode(Frame frame, EntityRef bullet, EntityRef robot)
    {
      var bulletTransform = frame.Get<Transform2D>(bullet);
      var hits = frame.Physics2D.OverlapShape(bulletTransform, ExplosionShape.CreateShape(frame));
      for (int i = 0; i < hits.Count; i++)
      {
        EntityRef entity = hits[i].Entity;

        // Only consider robots for damage
        if (entity == EntityRef.None || frame.Has<Status>(entity) == false)
        {
          continue;
        }

        // Only deal damages to robots not behind walls 
        Transform2D currentBotTransform = frame.Get<Transform2D>(entity);
        if (LineOfSightHelper.HasLineOfSight(frame, bulletTransform.Position, currentBotTransform.Position) == false)
        {
          continue;
        }

        // Don't hit the target robot, we deal his damage in another place so it doesnt suffer falloff
        if (entity == robot)
        {
          continue;
        }

        FP distance = FPVector2.Distance(bulletTransform.Position, currentBotTransform.Position);
        FP damagePercentage = 1 - distance / (ExplosionShape.CircleRadius + ShapeConfig.CircleRadius);
        damagePercentage = FPMath.Clamp01(damagePercentage);
        FP damage = Damage * damagePercentage;

        frame.Signals.OnRobotHit(bullet, entity, damage);
      }
    }
  }
}