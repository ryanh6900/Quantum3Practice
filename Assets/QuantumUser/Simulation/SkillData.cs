namespace Quantum
{
  using Photon.Deterministic;
  public class SkillData : AssetObject
  {
    public AssetRef<EntityPrototype> SkillPrototype;
    public FP ActivationDelay;
    public FP Damage;
    public Shape2DConfig ShapeConfig;
  }
}
