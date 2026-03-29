using System.Collections.Generic;
using Scriprs.Service.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scriprs.View
{
  public class GameplayEntitiesDebugView : MonoBehaviour
  {
    [SerializeField] private Text _text;
    [SerializeField] private float _refreshInterval = 0.25f;

    private readonly List<IGameplayEntity> _activeEntities = new();

    private IGameplayEntityTracker _tracker;
    private float _timer;
    private string _lastRenderedText;

    [Inject]
    private void Construct(IGameplayEntityTracker tracker)
    {
      _tracker = tracker;
    }

    private void Awake()
    {
      if (_text == null)
        _text = GetComponent<Text>();
    }

    private void Update()
    {
      _timer += Time.unscaledDeltaTime;

      if (_timer < _refreshInterval)
        return;

      _timer = 0f;
      Refresh();
    }

    private void Refresh()
    {
      if (_text == null || _tracker == null)
        return;

      _tracker.GetActiveEntities(_activeEntities);

      string newText = $"Tracked: {_tracker.TotalCount} Active: {_activeEntities.Count}";

      if (newText == _lastRenderedText)
        return;

      _lastRenderedText = newText;
      _text.text = newText;
    }
  }
}
