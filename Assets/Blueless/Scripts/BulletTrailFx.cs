namespace Blueless
{
  using Quantum;
  using UnityEngine;
  using System.Collections;

  public sealed class BulletTrailFx : BulletFx
  {
    public float Duration = 1.0f;
    public float DestroyDelay = 0.2f;
    public TrailRenderer BulletTrail;
    public ParticleSystem BulletParticle;

    private void Awake()
    {
      BulletTrail.enabled = false;
      BulletTrail.Clear();
    }

    public override unsafe void OnFx(Quantum.EntityRef robotRef, Vector3 position, Vector3 direction)
    {
      if (robotRef == EntityRef.None)
      {
        return;
      }

      var f = QuantumRunner.Default.Game.Frames.Predicted;
      var weaponInventory = f.Get<WeaponInventory>(robotRef);
      var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
      var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

      var player = f.Get<PlayerLink>(robotRef);
      var robotTransform = f.Get<Transform2D>(robotRef);

      var fireSpotOffset = WeaponHelper.GetFireSpotWorldOffset(
        weaponData,
        f.GetPlayerInput(player.PlayerRef)->AimDirection
      );

      BulletTrail.transform.position = robotTransform.Position.ToUnityVector3() + fireSpotOffset.ToUnityVector3();
      BulletParticle.transform.position = robotTransform.Position.ToUnityVector3() + fireSpotOffset.ToUnityVector3();
      BulletTrail.Clear();
      StartCoroutine(MakeEffect(robotTransform.Position.ToUnityVector3() + fireSpotOffset.ToUnityVector3(), position));
    }

    IEnumerator MakeEffect(Vector3 startPosition, Vector3 finalPosition)
    {
      BulletTrail.Clear();
      BulletParticle.Simulate(0);
      BulletParticle.Play();
      float t = 0;
      while (t < Duration)
      {
        t += Time.deltaTime;
        BulletTrail.transform.position = Vector3.Lerp(startPosition, finalPosition, t / Duration);
        BulletParticle.transform.position = Vector3.Lerp(startPosition, finalPosition, t / Duration);
        yield return null;
        BulletTrail.enabled = true;
      }

      BulletTrail.transform.position = finalPosition;
      BulletParticle.transform.position = finalPosition;
      Destroy(gameObject, DestroyDelay);
    }
  }
}