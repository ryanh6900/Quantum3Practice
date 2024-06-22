namespace Blueless
{
  using Quantum;
  using UnityEngine;

  public class CharacterSelectButtonDelegate : MonoBehaviour
  {
    public UnityEngine.UI.Button SelectButton;
    public AssetRef<EntityPrototype> CharacterPrototype;
    public CharacterSelectionUIController CharacterSelectionUIController;

    private void Start()
    {
      SelectButton.onClick.AddListener(OnCharacterSelected);
    }

    public void OnCharacterSelected()
    {
      CharacterSelectionUIController.OnSelectButtonClicked(CharacterPrototype);
    }
  }
}