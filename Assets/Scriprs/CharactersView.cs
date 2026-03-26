using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICharacter
{
    int Value { get; }
}

public class Character : ICharacter
{
    public int Value { get; private set; }

    public Character(int value)
    {
        Value = value;
    }
}

public class CharactersView : MonoBehaviour
{
    [SerializeField] private List<Character> _characters = new();
    [SerializeField] private Text _text;
    [SerializeField] private float _updateInterval = 0.25f;

    private float _timer;
    private string _lastRenderedText;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<Text>();
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;

        if (_timer < _updateInterval)
            return;

        _timer = 0f;
        RefreshView();
    }

    private void RefreshView()
    {
        if (_text == null)
            return;

        int activeCount = 0;
        float totalValue = 0f;

        for (int i = 0; i < _characters.Count; i++)
        {
            Character character = _characters[i];

            if (character == null)
                continue;

            activeCount++;
            totalValue += character.Value;
        }

        float averageValue = activeCount > 0 ? totalValue / activeCount : 0f;

        string newText = $"Characters: {activeCount} Avg value: {averageValue:F2}";

        if (newText == _lastRenderedText)
            return;

        _lastRenderedText = newText;
        _text.text = newText;
    }
}