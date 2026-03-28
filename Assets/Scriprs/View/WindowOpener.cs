using System;
using Scriprs.Service.StaticData;
using Scriprs.Service.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WindowOpener : MonoBehaviour
{
    private static IWindowService _windowService;
    private static IStaticDataService _staticDataService;
    public Button OpenButton;

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
        OpenButton.onClick.AddListener(OpenWindow);
       
    }

    private void OpenWindow()
    {
        Debug.Log("Open Window");
        _windowService.Open(WindowId.GameOverWindow);
    }
}
