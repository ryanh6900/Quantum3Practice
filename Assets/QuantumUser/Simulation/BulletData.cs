namespace Quantum
{
  using Photon.Deterministic;
  using UnityEngine;

  /// <summary>
  /// Polymorphic data asset for bullets
  /// </summary>
  public abstract class BulletData : AssetObject
  {
#if QUANTUM_UNITY
    [Header("View Configuration", order = 9)]
    public GameObject BulletDestroyFxGameObject;
    public Blueless.AudioConfiguration BulletDestroyAudioInfo;
#endif
    
    public AssetRef<EntityPrototype> BulletPrototype;
    public Shape2DConfig ShapeConfig;
    public FP Damage;
    public FP Range;
    public FP Duration;

    /// <summary>
    /// Post hit effects should happen on this function call.
    /// 
    /// Eg.: Explosions, damage calculations, etc.
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="bullet">The bullet that triggered the function call</param>
    /// <param name="targetRobot">The target of the bullet (is null when hitting a static collider)</param>
    public virtual unsafe void BulletAction(Frame frame, EntityRef bullet, EntityRef targetRobot)
    {
    }
  }
}