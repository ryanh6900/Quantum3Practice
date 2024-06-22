namespace Quantum
{
  using Photon.Deterministic;

  public static unsafe class WeaponHelper
  {
    public static FPVector2 GetFireSpotWorldOffset(WeaponData weaponData, FPVector2 direction)
    {
      FPVector2 positionOffset = weaponData.PositionOffset;
      FPVector2 firespotVector = weaponData.FireSpotOffset;
      return positionOffset + direction + firespotVector;
    }
  }
}