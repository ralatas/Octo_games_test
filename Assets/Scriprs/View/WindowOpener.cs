using System;
using Scriprs.Service.StaticData;
using Scriprs.Service.Windows;
using Scriprs.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WindowOpener : MonoBehaviour
{
    private static IWindowService _windowService;
    private static IStaticDataService _staticDataService;
    public Button openButton;
    public Button OpenCustomButton;

    [Inject]
    private void Construct( IWindowService windowService, IStaticDataService  staticDataService)
    {
        _staticDataService = staticDataService;
        _windowService = windowService;
    }

    private void Awake()
    {
        _staticDataService.LoadAll();
    }

    void Start()
    {
        openButton.onClick.AddListener(OpenWindow);
        OpenCustomButton.onClick.AddListener(OpenCustomWindow);
    }

    private void OpenCustomWindow()
    {
        Debug.Log("Open Window with payload");
        _windowService.Open(
            WindowId.GameActionsWindow,
            new WindowWithParamsPayload(
                "Setting", 
                "Set Game params", 
                new[] {
                    new CustomButton("Close", () => { _windowService.Close(WindowId.GameActionsWindow); }),
                    new CustomButton("Log",() => { Debug.Log("Log"); }),
                }));
    }

    private void OpenWindow()
    {
        Debug.Log("Open Simple Window");
        _windowService.Open(WindowId.GameOverWindow);
    }
}
