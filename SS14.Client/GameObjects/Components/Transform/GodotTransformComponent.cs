using SS14.Client.Graphics.ClientEye;
using SS14.Client.Interfaces;
using SS14.Client.Interfaces.GameObjects.Components;
using SS14.Client.Utility;
using SS14.Shared.Interfaces.GameObjects.Components;
using SS14.Shared.IoC;
using SS14.Shared.Maths;

namespace SS14.Client.GameObjects
{
    abstract public class GodotTransformComponent : ClientTransformComponent, IGodotTransformComponent
    {
        abstract public Godot.Node Node { get; }

        IGodotTransformComponent IGodotTransformComponent.Parent => (IGodotTransformComponent)Parent;

        abstract public Godot.Vector2 GlobalPosition { get; }

        protected override void SetPosition(float positionx, float positiony, float positionz = 0)
        {
            base.SetPosition(positionx, positiony, positionz);
            //SceneNode.Position = (position * EyeManager.PIXELSPERMETER).Rounded().Convert();
        }

        abstract protected void UpdateSceneVisibility();

        protected override void AttachParent(ITransformComponent parent)
        {
            if (parent == null)
            {
                return;
            }

            base.AttachParent(parent);
            Node.GetParent().RemoveChild(Node);
            ((IGodotTransformComponent)parent).Node.AddChild(Node);
            UpdateSceneVisibility();
        }

        protected override void DetachParent()
        {
            if (Parent == null)
            {
                return;
            }

            ((IGodotTransformComponent)Parent).Node.RemoveChild(Node);
            base.DetachParent();
            var holder = IoCManager.Resolve<ISceneTreeHolder>();
            holder.WorldRoot.AddChild(Node);
            UpdateSceneVisibility();
        }

        public override void OnAdd()
        {
            base.OnAdd();
            var holder = IoCManager.Resolve<ISceneTreeHolder>();
            holder.WorldRoot.AddChild(Node);
        }

        public override void OnRemove()
        {
            base.OnRemove();

            Node.QueueFree();
            Node.Dispose();
        }
    }
}
