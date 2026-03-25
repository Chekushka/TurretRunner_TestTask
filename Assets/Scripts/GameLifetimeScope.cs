using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<GameGameInputProvider>().As<IGameInputProvider>();
            builder.RegisterComponentInHierarchy<CarMovement>();
            builder.RegisterComponentInHierarchy<CameraController>();
            builder.RegisterComponentInHierarchy<RoadGenerator>();
        }
}