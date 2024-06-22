namespace Blueless
{
  using Quantum;
  using UnityEngine;
  using UnityEngine.UI;

  public class ChangeWeaponHud : QuantumSceneViewComponent<CustomViewContext>
  {
    public Image WeaponIconImage;

    public override void OnUpdateView()
    {
      var localPlayerView = ViewContext.LocalPlayerView;
      if (localPlayerView == null)
      {
        return;
      }
      var frame = localPlayerView.PredictedFrame;

      var wInventory = frame.Get<WeaponInventory>(localPlayerView.EntityRef);
      var weapon = wInventory.Weapons[wInventory.CurrentWeaponIndex];
      var weaponData = frame.FindAsset<WeaponData>(weapon.WeaponData.Id);
      
      WeaponIconImage.sprite = weaponData.UIIcon;
    }
  }
}