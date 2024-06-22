using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EventSystemController : MonoBehaviour
{
  void Awake()
  {
    EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
    if (eventSystems.Length == 0)
    {
      gameObject.AddComponent<EventSystem>();
      gameObject.AddComponent<InputSystemUIInputModule>();
    }
  }
}