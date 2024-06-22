using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
  public Transform RightHand;
  public Transform LeftHand;
  public Transform LookDir;
  public ParticleSystem Muzzle;

  public void ShootFx()
  {
    Muzzle.Stop();
    Muzzle.Play();
  }
}
