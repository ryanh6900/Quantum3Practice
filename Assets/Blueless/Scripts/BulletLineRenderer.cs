namespace Blueless
{
  using Quantum;
  using UnityEngine;

  public class BulletLineRenderer : QuantumEntityViewComponent
  {
    public float Lenght = 1;

    private LineRenderer _lineRenderer;
    private Vector3 _lastPos;

    public override void OnInitialize()
    {
      _lineRenderer = GetComponent<LineRenderer>();
    }

    public override void OnActivate(Frame frame)
    {
      _lineRenderer.SetPosition(0, transform.position);
      _lineRenderer.SetPosition(1, transform.position);
      _lastPos = transform.position;
    }

    public override void OnUpdateView()
    {
      var direction = Vector3.Normalize(transform.position - _lastPos);
      _lineRenderer.SetPosition(0, transform.position + direction / Lenght);
      _lineRenderer.SetPosition(1, transform.position + direction / Lenght - direction * Lenght);
      _lastPos = transform.position;
    }
  }
}