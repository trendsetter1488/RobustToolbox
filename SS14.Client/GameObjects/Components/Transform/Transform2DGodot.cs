using SS14.Shared.Maths;

namespace SS14.Client.GameObjects.Components.Transform
{
    public class Transform2DGodot : GodotTransformComponent
    {
        public Godot.Node2D SceneNode { get; private set; }

        public override Godot.Node Node => SceneNode;

        public override Godot.Vector2 GlobalPosition => throw new System.NotImplementedException();

        public override void OnAdd()
        {
            SceneNode = new Godot.Node2D
            {
                Name = $"Transform {Owner.Uid} ({Owner.Name})",
                Rotation = -MathHelper.PiOver2
            };
            base.OnAdd();
        }

        protected override void UpdateSceneVisibility()
        {
            SceneNode.Visible = IsMapTransform;
        }

        protected override void SetRotation(Angle rotationx, Angle rotationy = new Angle(), Angle rotationz = new Angle())
        {
            base.SetRotation(rotationx);
            SceneNode.Rotation = (float)rotationx - MathHelper.PiOver2;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            SceneNode = null;
        }
    }
}
