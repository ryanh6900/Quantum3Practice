namespace Quantum
{
  using Photon.Deterministic;

  public static unsafe class LineOfSightHelper
  {
    // Returns true if there's no static collider between source and target
    public static bool HasLineOfSight(Frame frame, FPVector2 source, FPVector2 target)
    {
      Physics2D.HitCollection hits = frame.Physics2D.LinecastAll(source, target, -1, QueryOptions.HitStatics);
      for (int i = 0; i < hits.Count; i++)
      {
        EntityRef entity = hits[i].Entity;
        if (entity == EntityRef.None)
        {
          return false;
        }
      }
      return true;
    }
  }
}