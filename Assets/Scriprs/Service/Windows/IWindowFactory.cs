using UnityEngine;

namespace Scriprs.Service.Windows
{
  public interface IWindowFactory
  {
    public void SetUIRoot(RectTransform uiRoot);
    public BaseWindow CreateWindow(WindowId windowId);
  }
}