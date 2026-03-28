using Scriprs.Service.Windows;
using UnityEngine;
using Zenject;

namespace Scriprs.Installers
{
    public class UIInitializer : MonoBehaviour, IInitializable
    {
        private IWindowFactory _windowFactory;
    
        public RectTransform UIRoot;

        [Inject]
        private void Construct(IWindowFactory windowFactory)
        {
            Debug.Log($"Initializing UIRoot1");
            _windowFactory = windowFactory;
        }
        
        public void Initialize()
        {
            Debug.Log($"Initializing UIRoot");
            _windowFactory.SetUIRoot(UIRoot);
        }

        
    }
}