using System;

namespace Blueless
{
  using Quantum;
  using UnityEngine;

  public class MenuCameraController : MonoBehaviour
  {
    private Camera _camera;

    private void Start()
    {
      _camera = GetComponent<Camera>();
    }

    void Update()
    {
      _camera.enabled = QuantumRunner.Default == null;
    }
  }
}