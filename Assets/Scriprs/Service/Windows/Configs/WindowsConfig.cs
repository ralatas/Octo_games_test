using System.Collections.Generic;
using UnityEngine;

namespace Scriprs.Service.Windows.Configs
{
  [CreateAssetMenu(fileName = "windowConfig", menuName = "ECS Survivors/Window Config")]
  public class WindowsConfig : ScriptableObject
  {
    public List<WindowConfig> WindowConfigs;
  }
}