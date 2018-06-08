using SS14.Client.Interfaces.GameObjects.Components;
using SS14.Shared.Interfaces.GameObjects.Components;

namespace SS14.Client.GameObjects
{
    public class GodotComponentFactory : ClientComponentFactory
    {
        public GodotComponentFactory() : base()
        {
            //TODO: FIX THISSSSSSSSSSSSSSSSS
            //Register<GodotTransformComponent>(overwrite: true);
            //RegisterReference<GodotTransformComponent, ITransformComponent>();
            //RegisterReference<GodotTransformComponent, IGodotTransformComponent>();
        }
    }
}
