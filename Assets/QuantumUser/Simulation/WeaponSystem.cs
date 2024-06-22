namespace Quantum
{
  using Photon.Deterministic;

  /// <summary>
  /// Handles all things weapon related
  ///   Things this system handles:
  ///   - Weapon ammo recharge 
  ///   - Firing bullets
  /// </summary>
  public unsafe class WeaponSystem : SystemMainThreadFilter<WeaponSystem.Filter>, ISignalOnRobotRespawn,
    ISignalOnGameEnded
  {
    public struct Filter
    {
      public EntityRef Entity;
      public PlayerLink* PlayerLink;
      public Status* Status;
      public WeaponInventory* WeaponInventory;
    }

    void ISignalOnGameEnded.OnGameEnded(Frame frame, GameController* gameController)
    {
      frame.SystemDisable<WeaponSystem>();
    }

    void ISignalOnRobotRespawn.OnRobotRespawn(Frame frame, EntityRef robot)
    {
      WeaponInventory* weaponInventory = frame.Unsafe.GetPointer<WeaponInventory>(robot);

      for (var i = 0; i < weaponInventory->Weapons.Length; i++)
      {
        Weapon* weapon = weaponInventory->Weapons.GetPointer(i);
        var weaponData = frame.FindAsset<WeaponData>(weapon->WeaponData.Id);

        weapon->IsRecharging = false;
        weapon->CurrentAmmo = weaponData.MaxAmmo;
        weapon->FireRateTimer = FP._0;
        weapon->DelayToStartRechargeTimer = FP._0;
        weapon->RechargeRate = FP._0;
      }
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

      Weapon* currentWeapon = weaponInventory->Weapons.GetPointer(weaponInventory->CurrentWeaponIndex);
      currentWeapon->FireRateTimer -= frame.DeltaTime;
      currentWeapon->DelayToStartRechargeTimer -= frame.DeltaTime;
      currentWeapon->RechargeRate -= frame.DeltaTime;

      WeaponData weaponData = frame.FindAsset<WeaponData>(currentWeapon->WeaponData.Id);
      if (currentWeapon->DelayToStartRechargeTimer < 0 && currentWeapon->RechargeRate <= 0 &&
          currentWeapon->CurrentAmmo < weaponData.MaxAmmo)
      {
        IncreaseAmmo(frame, currentWeapon, weaponData);
      }

      if (currentWeapon->FireRateTimer <= FP._0 && !currentWeapon->IsRecharging && currentWeapon->CurrentAmmo > 0)
      {
        Input* i = frame.GetPlayerInput(playerLink->PlayerRef);

        if (i->Fire.IsDown)
        {
          SpawnBullet(frame, robot, currentWeapon, i->AimDirection);
          currentWeapon->FireRateTimer = FP._1 / weaponData.FireRate;
          currentWeapon->ChargeTime = FP._0;
        }
      }
    }

    private static void IncreaseAmmo(Frame frame, Weapon* weapon, WeaponData data)
    {
      weapon->RechargeRate = data.RechargeTimer / (FP)data.MaxAmmo;
      weapon->CurrentAmmo++;

      if (weapon->CurrentAmmo == data.MaxAmmo)
      {
        weapon->IsRecharging = false;
      }
    }

    private static void SpawnBullet(Frame frame, EntityRef robot, Weapon* weapon, FPVector2 direction)
    {
      weapon->CurrentAmmo -= 1;
      if (weapon->CurrentAmmo == 0)
      {
        weapon->IsRecharging = true;
        weapon->DelayToStartRechargeTimer = -1;
      }

      WeaponData weaponData = frame.FindAsset<WeaponData>(weapon->WeaponData.Id);
      weapon->DelayToStartRechargeTimer = weaponData.TimeToRecharge;

      frame.Events.OnWeaponShoot(robot);

      BulletData bulletData = frame.FindAsset<BulletData>(weaponData.BulletData.Id);

      EntityPrototype prototypeAsset =
        frame.FindAsset<EntityPrototype>(new AssetGuid(bulletData.BulletPrototype.Id.Value));
      EntityRef bullet = frame.Create(prototypeAsset);

      BulletFields* bulletFields = frame.Unsafe.GetPointer<BulletFields>(bullet);
      Transform2D* bulletTransform = frame.Unsafe.GetPointer<Transform2D>(bullet);

      bulletFields->BulletData = bulletData;
      Transform2D* robotTransform = frame.Unsafe.GetPointer<Transform2D>(robot);

      var fireSpotWorldOffset = WeaponHelper.GetFireSpotWorldOffset(
        frame.FindAsset<WeaponData>(weapon->WeaponData.Id),
        direction
      );

      bulletTransform->Position = robotTransform->Position + fireSpotWorldOffset;

      bulletFields->Direction = direction * weaponData.ShootForce;
      bulletFields->Source = robot;
      bulletFields->Time = FP._0;
    }
  }
}