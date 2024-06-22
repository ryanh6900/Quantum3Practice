namespace Blueless
{
  using UnityEngine;
  using Photon.Deterministic;
  using Quantum;

  public unsafe class UnparentTrailOnDestroy : MonoBehaviour
  {
    public ParticleSystem Effect;
    public QuantumEntityView BulletView;
    private EntityRef _robotEntityRef;
    private Transform _parent;

    private void Start()
    {
      QuantumEvent.Subscribe<EventOnBulletDestroyed>(this, OnBulletDestroyed);

      var f = QuantumRunner.Default.Game.Frames.Verified;
      BulletView = GetComponentInParent<QuantumEntityView>();
      if (f.Exists(BulletView.EntityRef))
      {
        var bulletFields = f.Get<BulletFields>(BulletView.EntityRef);
        _robotEntityRef = bulletFields.Source;
        _parent = transform.parent;
        transform.parent = null;
      }
    }

    private void Update()
    {
      if (_parent != null)
      {
        transform.position = _parent.position;
      }
    }

    private void OnBulletDestroyed(EventOnBulletDestroyed eventData)
    {
      if (_robotEntityRef != eventData.Robot)
      {
        return;
      }

      Effect.Stop();

      Vector3 position = eventData.BulletPosition.ToUnityVector3();
      Effect.transform.position = position;
    }

    private void OnDestroy()
    {
      QuantumEvent.UnsubscribeListener(this);
    }
  }
}