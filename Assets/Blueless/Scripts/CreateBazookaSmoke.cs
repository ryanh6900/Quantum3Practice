namespace Blueless
{
using UnityEngine;
  public class CreateBazookaSmoke : MonoBehaviour
  {
    public GameObject SmokePrefab;

    public void OnCreateSmoke()
    {
      Instantiate(SmokePrefab, transform.parent);
    }
  }
}