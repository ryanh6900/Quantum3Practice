namespace Blueless
{
  using Quantum;
  using UnityEngine;

  public class RobotView : QuantumEntityViewComponent<CustomViewContext>
  {
    public Transform Body;
    public Animator CharacterAnimator;
    public Vector3 RightRotation;
    public Vector3 LeftRotation;
    public int LookDirection;

    public override void OnActivate(Frame frame)
    {
      PlayerLink playerLink = VerifiedFrame.Get<PlayerLink>(EntityRef);
      if (Game.PlayerIsLocal(playerLink.PlayerRef))
      {
        ViewContext.LocalPlayerView = this;
      }
    }

    public override void OnUpdateView()
    {
      if (CharacterAnimator.GetBool("IsFacingRight"))
      {
        Body.localRotation = Quaternion.Euler(RightRotation);
        LookDirection = 1;
      }
      else
      {
        Body.localRotation = Quaternion.Euler(LeftRotation);
        LookDirection = -1;
      }
    }
  }
}