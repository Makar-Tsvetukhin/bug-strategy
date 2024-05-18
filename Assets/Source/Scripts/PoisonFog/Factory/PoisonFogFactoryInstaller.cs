using Zenject;

namespace PoisonFog.Factory
{
    public class PoisonFogFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            PoisonFogFactory poisonFogFactory = FindObjectOfType<PoisonFogFactory>();
            Container.Bind<PoisonFogFactory>().FromInstance(poisonFogFactory).AsSingle();
        }
    }
}