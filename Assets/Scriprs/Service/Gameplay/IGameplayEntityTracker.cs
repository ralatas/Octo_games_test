using System.Collections.Generic;

namespace Scriprs.Service.Gameplay
{
  public interface IGameplayEntityTracker
  {
    int TotalCount { get; }
    void Register(IGameplayEntity entity);
    void Unregister(IGameplayEntity entity);
    void GetActiveEntities(List<IGameplayEntity> results);
  }
}
