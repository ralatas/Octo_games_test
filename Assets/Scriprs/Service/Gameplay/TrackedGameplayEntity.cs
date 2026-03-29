using UnityEngine;
using Zenject;

namespace Scriprs.Service.Gameplay
{
  public class TrackedGameplayEntity : MonoBehaviour, IGameplayEntity
  {
    private IGameplayEntityTracker _tracker;

    public GameObject GameObject => gameObject;
    public virtual bool IsActive => isActiveAndEnabled;

    [Inject]
    private void Construct(IGameplayEntityTracker tracker)
    {
      _tracker = tracker;
    }

    protected virtual void OnEnable()
    {
      _tracker?.Register(this);
    }

    protected virtual void OnDestroy()
    {
      _tracker?.Unregister(this);
    }
  }
}
