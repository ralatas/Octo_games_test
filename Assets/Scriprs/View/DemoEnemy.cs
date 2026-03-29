using Scriprs.Service.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class DemoEnemy : TrackedGameplayEntity
{
    [SerializeField] private Button DeactivateButton;
    [SerializeField] private Button RemoveButton;
    public bool IsDead;
    public override bool IsActive => base.IsActive && !IsDead;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        DeactivateButton.onClick.AddListener(() =>
        {
            IsDead = true;
        });
        RemoveButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DeactivateButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.RemoveAllListeners();
    }
    
}
