namespace Quantum
{
  using Photon.Deterministic;
  /// <summary>
  /// Normal bullet behavior
  /// 
  /// Deals damage on a robot.
  /// </summary>
  [System.Serializable]
  public class BulletDataCommon : BulletData
  {
    public override unsafe void BulletAction(Frame frame, EntityRef bullet, EntityRef targetRobot)
    {
      if (targetRobot != EntityRef.None)
      {
        frame.Signals.OnRobotHit(bullet, targetRobot, Damage);
      }

      BulletFields fields = frame.Get<BulletFields>(bullet);
      FPVector2 position = frame.Get<Transform2D>(bullet).Position;
      frame.Events.OnBulletDestroyed(bullet.GetHashCode(), fields.Source, position, fields.Direction, fields.BulletData);
      frame.Destroy(bullet);
    }
  }
}