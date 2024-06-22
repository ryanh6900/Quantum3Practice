namespace Quantum
{
  using Photon.Deterministic;
  public class SkillInventoryData : AssetObject
  {
    public FP CastRate;
    public FP CastForce;
    public AssetRef<SkillData> SkillData;
  }
}
