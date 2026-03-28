using System.Collections.Generic;
using UnityEngine;

namespace Scriprs.Service.Windows
{
  public class WindowService : IWindowService
  {
    private readonly IWindowFactory _windowFactory;

    private readonly List<BaseWindow> _openedWindows = new();

    public WindowService(IWindowFactory windowFactory) =>
      _windowFactory = windowFactory;

    public void Open(WindowId windowId) => 
      _openedWindows.Add(_windowFactory.CreateWindow(windowId));
    public void Open(WindowId windowId, IWindowPayload payload) => 
      _openedWindows.Add(_windowFactory.CreateWindow(windowId, payload));

    public void Close(WindowId windowId)
    {
      BaseWindow window = _openedWindows.Find(x => x.Id == windowId);
      
      _openedWindows.Remove(window);
      
      GameObject.Destroy(window.gameObject);
    }
  }
}