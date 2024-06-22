namespace Blueless
{
  using TMPro;
  using Quantum;
  using UnityEngine;

  public unsafe class PlayerUI : QuantumEntityViewComponent
  {
    [Header("References")] public SpriteRenderer Health;
    public SpriteRenderer WeaponAmmo;
    public SpriteRenderer WeaponFireRate;

    public TMP_Text PlayerName;

    [Header("Configurations")] public Color EnemyColor;
    public Color WeaponColor;

    public override void OnActivate(Frame frame)
    {
      var player = VerifiedFrame.Get<PlayerLink>(EntityRef).PlayerRef;
      PlayerName.text = VerifiedFrame.GetPlayerData(player).PlayerNickname;

      if (Game.PlayerIsLocal(player) == false)
      {
        Health.color = EnemyColor;
        PlayerName.color = EnemyColor;
      }
    }

    public override void OnUpdateView()
    {
      var weaponInventory = VerifiedFrame.Get<WeaponInventory>(EntityRef);
      var currentWeapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
      var weaponData = VerifiedFrame.FindAsset<WeaponData>(currentWeapon.WeaponData.Id);

      var status = VerifiedFrame.Get<Status>(EntityRef);
      var statusData = VerifiedFrame.FindAsset<StatusData>(status.StatusData.Id);


      var healthRatio = (status.CurrentHealth / statusData.MaxHealth).AsFloat;
      var ammoRatio = (float)currentWeapon.CurrentAmmo / weaponData.MaxAmmo;
      var fireRateRatio = Mathf.Clamp01((float)currentWeapon.FireRateTimer / weaponData.FireRate.AsFloat);

      Health.size = new Vector2(healthRatio * 2, 0.25f);
      WeaponAmmo.size = new Vector2(ammoRatio * 2, 0.25f);
      WeaponFireRate.size = new Vector2((1 - fireRateRatio) * 2, 0.10f);

      WeaponColor.a = currentWeapon.IsRecharging ? 0.5f : 1;
      WeaponAmmo.color = WeaponColor;
    }
  }
}