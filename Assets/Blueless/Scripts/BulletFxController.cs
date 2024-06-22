﻿namespace Blueless
{
  using UnityEngine;
  using Quantum;
  using System.Collections.Generic;

  /// <summary>
  /// This Behavior handles events related to bullets, such as destruction effects and explosions
  /// </summary>
  public sealed class BulletFxController : MonoBehaviour
  {
    private Dictionary<EntityRef, int> _robotsLastBulletHash = new Dictionary<EntityRef, int>();

    private void Awake()
    {
      QuantumEvent.Subscribe<EventOnBulletDestroyed>(this, OnBulletDestroyed);
    }

    private void OnDestroy()
    {
      QuantumEvent.UnsubscribeListener(this);
    }

    private unsafe void OnBulletDestroyed(EventOnBulletDestroyed eventData)
    {
      int lastBullet;
      int currentBullet = eventData.BulletRefHashCode;
      if (_robotsLastBulletHash.TryGetValue(eventData.Robot, out lastBullet) && currentBullet == lastBullet)
      {
        return;
      }

      Vector3 position = eventData.BulletPosition.ToUnityVector3();
      Vector3 direction = eventData.BulletDirection.ToUnityVector3();

      var bulletData = QuantumUnityDB.GetGlobalAsset<BulletData>(eventData.BulletData);
      var fxPrefab = bulletData.BulletDestroyFxGameObject.GetComponent<BulletFx>();
      var fx = Instantiate(fxPrefab, position, Quaternion.LookRotation(-direction));
      fx.OnFx(eventData.Robot, position, direction);

      _robotsLastBulletHash[eventData.Robot] = currentBullet;
    }
  }
}