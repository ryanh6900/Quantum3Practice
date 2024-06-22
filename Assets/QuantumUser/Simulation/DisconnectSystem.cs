namespace Quantum
{
  using Photon.Deterministic;

  public unsafe class DisconnectSystem : SystemMainThreadFilter<DisconnectSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public PlayerLink* PlayerLink;
      public Status* Status;
    }

    public override void Update(Frame frame, ref Filter filter)
    {
      var playerID = filter.PlayerLink;
      var robotStatus = filter.Status;

      if (frame.IsPredicted) return;

      DeterministicInputFlags flags = frame.GetPlayerInputFlags(playerID->PlayerRef);

      if ((flags & DeterministicInputFlags.PlayerNotPresent) == DeterministicInputFlags.PlayerNotPresent)
      {
        robotStatus->DisconnectedTicks++;
      }
      else
      {
        robotStatus->DisconnectedTicks = 0;
      }

      if (robotStatus->DisconnectedTicks >= 15)
      {
        frame.Destroy(filter.Entity);
      }
    }
  }
}