namespace Quantum
{
  using Photon.Deterministic;

  /// <summary>
  /// Handles changing weapon
  /// </summary>
  public unsafe class WeaponInventorySystem : SystemMainThreadFilter<WeaponInventorySystem.Filter>, ISignalOnGameEnded
  {
    public struct Filter
    {
      public EntityRef Entity;
      public PlayerLink* PlayerLink;
      public Status* Status;
      public WeaponInventory* WeaponInventory;
    }

    public override void Update(Frame frame, ref Filter filter)
    {
      var robot = filter.Entity;
      var playerLink = filter.PlayerLink;
      var status = filter.Status;
      var weaponInventory = filter.WeaponInventory;

      if (status->IsDead)
      {
        return;
      }

      Input* input = frame.GetPlayerInput(playerLink->PlayerRef);
      if (input->ChangeWeapon.WasPressed)
      {
        ChangeWeapon(frame, robot, weaponInventory);
      }
    }

    private void ChangeWeapon(Frame frame, EntityRef robot, WeaponInventory* weaponInventory)
    {
      weaponInventory->CurrentWeaponIndex = (weaponInventory->CurrentWeaponIndex + 1) % weaponInventory->Weapons.Length;
      Weapon* currentWeapon = weaponInventory->Weapons.GetPointer(weaponInventory->CurrentWeaponIndex);
      currentWeapon->ChargeTime = FP._0;

      frame.Events.OnRobotChangeWeapon(robot);
    }

    void ISignalOnGameEnded.OnGameEnded(Frame frame, GameController* gameController)
    {
      frame.SystemDisable<WeaponInventorySystem>();
    }
  }
}