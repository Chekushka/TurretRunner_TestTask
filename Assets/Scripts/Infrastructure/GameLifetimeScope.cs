using Core;
using Gameplay.Car;
using Gameplay.Enemies;
using Gameplay.Environment;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<GameInputProvider>().As<IGameInputProvider>();
            builder.RegisterComponentInHierarchy<CarMovement>().AsSelf();
            builder.RegisterComponentInHierarchy<CameraController>();
            builder.RegisterComponentInHierarchy<RoadGenerator>();
            builder.RegisterComponentInHierarchy<EnemySpawner>();
            builder.RegisterComponentInHierarchy<CarHealth>();
        }
    }
}