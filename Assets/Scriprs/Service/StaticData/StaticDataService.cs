using System;
using System.Collections.Generic;
using System.Linq;
using Scriprs.Service.Windows;
using Scriprs.Service.Windows.Configs;
using UnityEngine;

namespace Scriprs.Service.StaticData
{
  public interface IStaticDataService
  {
    void LoadAll();
    GameObject GetWindowPrefab(WindowId id);
  }

  public class StaticDataService : IStaticDataService
  {
    private Dictionary<WindowId, GameObject> _windowPrefabsById;
    
    public void LoadAll()
    {
      LoadWindows();
    }

    public GameObject GetWindowPrefab(WindowId id) =>
      _windowPrefabsById.TryGetValue(id, out GameObject prefab)
        ? prefab
        : throw new Exception($"Prefab config for window {id} was not found");
    
  
    private void LoadWindows()
    {
      _windowPrefabsById = Resources
        .Load<WindowsConfig>("Configs/Windows/windowsConfig")
        .WindowConfigs
        .ToDictionary(x => x.Id, x => x.Prefab);
      Debug.Log("Loaded " + _windowPrefabsById.Count);
    }
  }
}