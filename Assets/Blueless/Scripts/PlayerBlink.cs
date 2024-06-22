namespace Blueless
{
  using UnityEngine;
  using Quantum;

  /// <summary>
  /// Makes the player flash when receiving damage 
  /// </summary>
  public sealed unsafe class PlayerBlink : QuantumEntityViewComponent
  {
    public Material BlinkDamageMaterial;
    public Renderer BoyMesh;
    public Renderer GirlMesh;
    public float Time = 0.1f;
    private Material[] _originalMaterialsBoy;
    private Material[] _originalMaterialsGirl;
    private Material[] _blinkMaterialsBoy;
    private Material[] _blinkMaterialsGirl;

    public override void OnInitialize()
    {
      // Either subscribe during OnInitialize() and enabled onlyIfActiveAndEnabled
      // Or subscribe during OnActivate(), and unsubscribe during OnDeactivate()
      QuantumEvent.Subscribe<EventOnRobotBlink>(this, RobotBlink, onlyIfActiveAndEnabled: true);
      SetupMaterials();
    }

    private void SetupMaterials()
    {
      _blinkMaterialsBoy = new Material[BoyMesh.materials.Length];
      _blinkMaterialsGirl = new Material[GirlMesh.materials.Length];

      for (int i = 0; i < BoyMesh.materials.Length; i++)
      {
        _blinkMaterialsBoy[i] = BlinkDamageMaterial;
        _originalMaterialsBoy = BoyMesh.sharedMaterials;
      }

      for (int i = 0; i < GirlMesh.materials.Length; i++)
      {
        _blinkMaterialsGirl[i] = BlinkDamageMaterial;
        _originalMaterialsGirl = GirlMesh.sharedMaterials;
      }
    }

    private void RobotBlink(EventOnRobotBlink eventData)
    {
      // Is the blink event meant for this entity
      if (eventData.Robot.Equals(EntityRef))
      {
        StartCoroutine(Blink());
      }
    }

    System.Collections.IEnumerator Blink()
    {
      GirlMesh.materials = _blinkMaterialsGirl;
      BoyMesh.materials = _blinkMaterialsBoy;
      yield return new WaitForSeconds(Time);
      BoyMesh.materials = _originalMaterialsBoy;
      GirlMesh.materials = _originalMaterialsGirl;
    }
  }
}