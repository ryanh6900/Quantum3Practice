namespace Blueless
{
  using Quantum;
  using UnityEngine;
  using System.Collections;

  public unsafe class SkillView : MonoBehaviour
  {
    public ParticleSystem EffectExplosionPrefab;
    public ParticleSystem EffectHitPrefab;
    public ParticleSystem EffectPrefab;

    private void Start()
    {
      QuantumEvent.Subscribe<EventOnSkillHitTarget>(this, HitEffect);
      QuantumEvent.Subscribe<EventOnSkillActivated>(this, SkillActivated);
    }

    private void OnDestroy()
    {
      QuantumEvent.UnsubscribeListener(this);
    }

    private void SkillActivated(EventOnSkillActivated eventData)
    {
      Instantiate(EffectExplosionPrefab, eventData.SkillPosition.ToUnityVector3(), Quaternion.identity);
    }

    private void HitEffect(EventOnSkillHitTarget eventData)
    {
      Frame frame = eventData.Game.Frames.Predicted;
      var robotPosition = frame.Get<Transform2D>(eventData.Target).Position;
      var initialPosition = eventData.SkillPosition.ToUnityVector3();
      var finalPosition = robotPosition.ToUnityVector3();
      StartCoroutine(HitEffectCoroutine(initialPosition, finalPosition));
    }

    private IEnumerator HitEffectCoroutine(Vector3 initialPosition, Vector3 finalPosition)
    {
      var obj = Instantiate(EffectPrefab, initialPosition, Quaternion.identity);
      yield return null;
      obj.transform.position = finalPosition;
      Instantiate(EffectHitPrefab, finalPosition, Quaternion.identity);
    }
  }
}