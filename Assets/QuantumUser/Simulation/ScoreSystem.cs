namespace Quantum
{
  /// <summary>
  ///   Handles player scores (through signals)
  /// </summary>
  public unsafe class ScoreSystem : SystemSignalsOnly, ISignalOnRobotDeath
  {
    void ISignalOnRobotDeath.OnRobotDeath(Frame frame, EntityRef deadRef, EntityRef killerRef)
    {
      Score* killerScore = frame.Unsafe.GetPointer<Score>(killerRef);
      Score* deadScore = frame.Unsafe.GetPointer<Score>(deadRef);

      if (killerRef != deadRef)
      {
        killerScore->Kills += 1;
      }
      deadScore->Deaths += 1;
    }
  }
}