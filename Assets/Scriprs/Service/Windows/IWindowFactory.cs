using UnityEngine;

namespace Scriprs.Service.Windows
{
  public interface IWindowFactory
  {
    public void SetUIRoot(RectTransform uiRoot);
    public BaseWindow CreateWindow(WindowId windowId);
    public BaseWindow CreateWindow(WindowId windowId, IWindowPayload payload);
  }
  
  public interface IWindowPayload { }
}