namespace Blueless
{
  using Photon.Deterministic;
  using Quantum;
  using TMPro;
  using UnityEngine;
  using UnityEngine.UI;

  /// <summary>
  /// This behavior handles the Skill UI, showing cooldowns and etc
  /// </summary>
  public class SkillHud : QuantumSceneViewComponent<CustomViewContext>
  {
    public GameObject CooldownObject;
    public Image CooldownFill;
    public TMP_Text CooldownText;

    unsafe public override void OnUpdateView()
    {
      var localView = ViewContext.LocalPlayerView;
      if (localView == null)
      {
        return;
      }

      var frame = localView.PredictedFrame;
      var skillInventory = frame.Get<SkillInventory>(localView.EntityRef);
      var data = frame.FindAsset<SkillInventoryData>(skillInventory.SkillInventoryData.Id);
      var cooldownLeft = skillInventory.CastRateTimer;
      var cooldownMax = data.CastRate;

      CooldownObject.SetActive(cooldownLeft > FP._0);
      CooldownFill.fillAmount = (cooldownLeft / cooldownMax).AsFloat;
      CooldownText.text = Mathf.CeilToInt(cooldownLeft.AsFloat).ToString("0");
    }
  }
}
