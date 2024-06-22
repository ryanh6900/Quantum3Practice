namespace Quantum
{
  using Photon.Deterministic;

  public static unsafe class RespawnHelper
  {
    public static void RespawnRobot(Frame frame, EntityRef robot)
    {
      FPVector2 position = FPVector2.One * 4;
      int spawnCount = frame.ComponentCount<SpawnIdentifier>();
      if (spawnCount != 0)
      {
        int index = frame.RNG->Next(0, spawnCount);
        int count = 0;
        foreach (var (spawn, spawnIdentifier) in frame.Unsafe.GetComponentBlockIterator<SpawnIdentifier>())
        {
          if (count == index)
          {
            Transform2D spawnTransform = frame.Get<Transform2D>(spawn);
            position = spawnTransform.Position;
            break;
          }

          count++;
        }
      }

      Transform2D* robotTransform = frame.Unsafe.GetPointer<Transform2D>(robot);
      PhysicsCollider2D* collider = frame.Unsafe.GetPointer<PhysicsCollider2D>(robot);

      robotTransform->Position = position;
      collider->IsTrigger = false;

      frame.Signals.OnRobotRespawn(robot);
      frame.Events.OnRobotRespawn(robot);
    }
  }
}