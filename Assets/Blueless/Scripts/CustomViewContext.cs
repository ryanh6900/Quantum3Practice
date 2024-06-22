namespace Blueless
{
  using Photon.Deterministic;
  using Quantum;
  using UnityEngine;

  public class CustomViewContext : MonoBehaviour, IQuantumViewContext
  {
    public RobotView LocalPlayerView;
    public FPVector2 LocalPlayerLastDirection;

  }
}