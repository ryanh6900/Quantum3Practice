namespace Quantum
{
  using Photon.Deterministic;
  /// <summary>
  ///   Handles game timer
  /// </summary>
  public unsafe class GameControllerSystem : SystemMainThread, ISignalOnGameEnded
  {
    public override void OnInit(Frame frame)
    {
      frame.Global->GameController.GameTimer = FP._0;
      frame.Global->GameController.State = GameState.Running;
    }

    public override void Update(Frame frame)
    {
      GameControllerData gameConfigData = frame.FindAsset<GameControllerData>(frame.RuntimeConfig.GameConfigData.Id);
      if (frame.Global->GameController.GameTimer >= gameConfigData.GameDuration)
      {
        frame.Signals.OnGameEnded(&frame.Global->GameController);
        frame.Events.OnGameEnded();
      }
      else
      {
        frame.Global->GameController.GameTimer += frame.DeltaTime;
      }
    }

    void ISignalOnGameEnded.OnGameEnded(Frame frame, GameController* gameController)
    {
      frame.Global->GameController.State = GameState.Ended;
      frame.SystemDisable<GameControllerSystem>();
    }
  }
}