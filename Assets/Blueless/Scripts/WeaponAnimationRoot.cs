using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationRoot : MonoBehaviour
{
  public Transform SHRoot;
  public Vector3 WeaponOffset;

  private float _minShRoot = 0.7f;

  void Update()
  {
    WeaponOffset = new Vector3(0, SHRoot.localPosition.y - _minShRoot, 0);
  }
}
