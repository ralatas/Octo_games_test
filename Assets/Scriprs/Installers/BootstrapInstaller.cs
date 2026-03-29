using Scriprs.Service.Gameplay;
using Scriprs.Service.StaticData;
using Scriprs.Service.Windows;
using UnityEngine;
using Zenject;

namespace Scriprs.Installers
{
    public class BootstrapInstaller: MonoInstaller, IInitializable
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameplayEntityTracker>().To<GameplayEntityTracker>().AsSingle();
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            BindUIFactories();
        }
        private void BindUIFactories()
        {
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
        }
        public void Initialize()
        {
            //Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }
    }
}
