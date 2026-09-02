using Archeus.Core.Debugging;

namespace Archeus.Game.Bootstrap
{
    public class BattlePresentationWorldProcess : IBootstrapProcess
    {
        public int Order => PresentationBootstrapOrder.PresentationWorld;

        public void Initialise(WorldContext rootContext)
        {
            BattlePresentationWorld presentationWorld = new BattlePresentationWorld(rootContext);
            presentationWorld.Initialise();

            rootContext.Register<BattlePresentationWorld>(presentationWorld);

            Logging.Info(LogCategory.Setup, "Presentation world initialised.");
        }
    }
}
