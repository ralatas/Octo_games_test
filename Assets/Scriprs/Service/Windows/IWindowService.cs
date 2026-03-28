namespace Scriprs.Service.Windows
{
  public interface IWindowService
  {
    void Open(WindowId windowId);
    void Open(WindowId windowId, IWindowPayload  payload);
    void Close(WindowId windowId);
  }
}