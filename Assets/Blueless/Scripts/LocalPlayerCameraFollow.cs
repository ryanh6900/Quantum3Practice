namespace Blueless
{
  using Quantum;
  using UnityEngine;

  [RequireComponent(typeof(Camera))]
  public unsafe class LocalPlayerCameraFollow : QuantumSceneViewComponent<CustomViewContext>
  {
    public float SmoothTime = 0.3f;
    public float MaxSpeed = 10.0f;
    public float LookOffset = 10.0f;
    public float ZSmoothTime = 0.5f;

    private Vector2 _currentVelocity;
    private float _originalDistance;
    private float _zVelocity = 0.0f;
    private float _zDistance = 0.0f;
    private Camera _localCamera;

    public override void OnInitialize()
    {
      _localCamera = GetComponent<Camera>();
      _originalDistance = _localCamera.transform.position.z;
    }

    public override void OnUpdateView()
    {
      if (ViewContext.LocalPlayerView == null) {
        return;
      }

      Vector2 cameraPosition = _localCamera.transform.position;
      Vector2 targetPosition = ViewContext.LocalPlayerView.transform.position;
      targetPosition.x += LookOffset * ViewContext.LocalPlayerView.LookDirection;
      cameraPosition = Vector2.SmoothDamp(cameraPosition, targetPosition, ref _currentVelocity, SmoothTime, MaxSpeed,
        Time.deltaTime);
      
      var targetDistance = 0.0f;
      _zDistance = Mathf.SmoothDamp(_zDistance, targetDistance, ref _zVelocity, ZSmoothTime);

      _localCamera.transform.position = new Vector3(
        cameraPosition.x,
        cameraPosition.y,
        _originalDistance - _zDistance
      );
    }
  }
}