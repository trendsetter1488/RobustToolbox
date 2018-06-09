using SS14.Client.GameObjects.Components.Transform;
using SS14.Client.Interfaces.GameObjects.Components;
using SS14.Shared.Interfaces.GameObjects.Components;

namespace SS14.Client.GameObjects
{
    public class GodotComponentFactory : ClientComponentFactory
    {
        public GodotComponentFactory() : base()
        {
            if(SceneTreeHolder.arewethreeD)
            {
                Register<Transform3DGodot>(overwrite: true);
                RegisterReference<Transform3DGodot, ITransformComponent>();
                RegisterReference<Transform3DGodot, IGodotTransformComponent>();
            }
            else
            {
                Register<Transform2DGodot>(overwrite: true);
                RegisterReference<Transform2DGodot, ITransformComponent>();
                RegisterReference<Transform2DGodot, IGodotTransformComponent>();
            }
        }
    }
}
