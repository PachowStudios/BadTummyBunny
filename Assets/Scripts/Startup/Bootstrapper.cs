using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
  private static Bootstrapper Instance { get; set; }

  private void Awake()
  {
    EnsureSingleInstance();

    Instance = this;

    SaveService.Load();
  }

  private void OnApplicationQuit()
    => SaveService.Save();

  private void EnsureSingleInstance()
  {
    if (FindObjectsOfType<Bootstrapper>().HasMultiple())
      this.DestroyGameObject();    
  }
}
