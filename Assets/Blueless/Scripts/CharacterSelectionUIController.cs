namespace Blueless
{
  using UnityEngine;
  using UnityEngine.UI;
  using System.Collections;
  using Quantum;

  public class CharacterSelectionUIController : MonoBehaviour
  {
    public GameObject TouchUI;
    public UnityEngine.UI.Button[] SelectButtons;

    private Animator _animator;
    private Coroutine _hideCoroutine;

    void Start()
    {
      SelectButtons = GetComponentsInChildren<UnityEngine.UI.Button>();
      TryGetComponent(out _animator);
      TouchUI.SetActive(false);
      QuantumEvent.Subscribe<EventOnPlayerSelectedCharacter>(this, OnPlayerSelected);
    }

    private void OnPlayerSelected(EventOnPlayerSelectedCharacter e)
    {
      if (e.Game.PlayerIsLocal(e.PlayerRef) == false)
      {
        return;
      }

      TouchUI.SetActive(true);

      if (_animator)
      {
        if (_hideCoroutine != null)
        {
          StopCoroutine(_hideCoroutine);
        }

        _hideCoroutine = StartCoroutine(HideAnimCoroutine());
        return;
      }
      else
      {
        gameObject.SetActive(false);
      }
    }

    public void OnSelectButtonClicked(AssetRef<EntityPrototype> characterPrototype)
    {
      QuantumRunner runner = QuantumRunner.Default;
      if (runner == null) return;

      RuntimePlayer playerData = new RuntimePlayer();
      playerData.PlayerAvatar = characterPrototype;
      
      var menu = FindObjectOfType<Quantum.Menu.QuantumMenuUIController>();
      if (menu != null)
      {
        playerData.PlayerNickname = menu.DefaultConnectionArgs.Username;
      }
      runner.Game.AddPlayer(playerData);

      foreach (var button in SelectButtons)
      {
        button.interactable = false;
      }
    }

    private IEnumerator HideAnimCoroutine()
    {
      _animator.Play("Hide");
      yield return null;
      while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
      {
        yield return null;
      }

      gameObject.SetActive(false);
    }
  }
}