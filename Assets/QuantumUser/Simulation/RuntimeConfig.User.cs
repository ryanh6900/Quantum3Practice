namespace Quantum
{
  using Photon.Deterministic;

  public partial class RuntimeConfig
  {
    public AssetRef<GameControllerData> GameConfigData;

    partial void SerializeUserData(BitStream stream)
    {
      stream.Serialize(ref GameConfigData);
    }
  }
}