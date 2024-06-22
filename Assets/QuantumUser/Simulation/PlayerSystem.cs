using UnityEngine;

namespace Quantum
{
  public unsafe class PlayerSystem : SystemSignalsOnly, ISignalOnPlayerAdded
  {
    public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
    {
      RuntimePlayer data = frame.GetPlayerData(player);

      if (data.PlayerAvatar != null)
      {
        SetPlayerCharacter(frame, player, data.PlayerAvatar);
      }
      else
      {
        Debug.LogWarning("Character prototype is null on RuntimePlayer, check QuantumMenuConnectionBehaviourSDK to prevent adding player automatically!");
      }
    }
    
    private void SetPlayerCharacter(Frame frame, PlayerRef player, AssetRef<EntityPrototype> prototypeAsset)
    {
      EntityRef character = frame.Create(prototypeAsset);

      PlayerLink* playerLink = frame.Unsafe.GetPointer<PlayerLink>(character);
      playerLink->PlayerRef = player;

      RespawnHelper.RespawnRobot(frame, character);
      frame.Events.OnRobotCreated(character);
      frame.Signals.OnRobotRespawn(character);
      
      frame.Events.OnPlayerSelectedCharacter(player);
    }
  }
}