using System.Collections.Generic;

namespace Scriprs.Service.Gameplay
{
  public class GameplayEntityTracker : IGameplayEntityTracker
  {
    private readonly List<IGameplayEntity> _entities = new();
    private readonly HashSet<IGameplayEntity> _entitySet = new();

    public int TotalCount => _entities.Count;

    public void Register(IGameplayEntity entity)
    {
      if (entity == null || !_entitySet.Add(entity))
        return;

      _entities.Add(entity);
    }

    public void Unregister(IGameplayEntity entity)
    {
      if (entity == null || !_entitySet.Remove(entity))
        return;

      _entities.Remove(entity);
    }

    public void GetActiveEntities(List<IGameplayEntity> results)
    {
      results.Clear();

      for (int i = _entities.Count - 1; i >= 0; i--)
      {
        IGameplayEntity entity = _entities[i];

        if (entity == null || entity.GameObject == null)
        {
          _entitySet.Remove(entity);
          _entities.RemoveAt(i);
          continue;
        }

        if (entity.IsActive)
          results.Add(entity);
      }
    }
  }
}
