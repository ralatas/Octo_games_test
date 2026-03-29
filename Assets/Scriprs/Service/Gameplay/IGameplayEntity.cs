using UnityEngine;

namespace Scriprs.Service.Gameplay
{
  public interface IGameplayEntity
  {
    GameObject GameObject { get; }
    bool IsActive { get; }
  }
}
