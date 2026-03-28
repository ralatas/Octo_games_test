using Scriprs.Service.Windows;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.GameOver.UI
{
  public class GameOverWindow : BaseWindow
  {
    public Button ReturnHomeButton;
    private IWindowService _windowService;

    [Inject]
    private void Construct( IWindowService windowService)
    {
      Id = WindowId.GameOverWindow;
      _windowService = windowService;
    }

    protected override void Initialize()
    {
      ReturnHomeButton.onClick.AddListener(ReturnHome);
    }

    private void ReturnHome()
    {
      _windowService.Close(Id);
      
      //Do something
    }
  }
}