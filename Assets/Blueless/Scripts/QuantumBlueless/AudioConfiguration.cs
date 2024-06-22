namespace Blueless
{
  using UnityEngine;
  using UnityEngine.Serialization;

  [System.Serializable]
  public struct AudioConfiguration
  {
    public AudioClip Clip;

    [Range(0, 1.0f)]
    public float Volume;

    public bool Is2D;
    public bool Loop;
    public float Delay;

    public string Name
    {
      get { return Clip == null ? "No Clip selected" : Clip.name; }
    }

    public bool IsValid()
    {
      return Clip != null;
    }

    public void AssignToAudioSource(AudioSource audioSource)
    {
      audioSource.volume = Volume;
      audioSource.clip = Clip;
      audioSource.spatialBlend = Is2D ? 0.0f : 1.0f;
      audioSource.loop = Loop;
    }
  }
}