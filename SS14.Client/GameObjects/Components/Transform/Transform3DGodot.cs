using Godot;
using SS14.Shared.Maths;

namespace SS14.Client.GameObjects.Components.Transform
{
    public class Transform3DGodot : GodotTransformComponent
    {
        public Godot.Spatial SceneNode { get; private set; }

        public override Godot.Node Node => SceneNode;

        public override Godot.Vector2 GlobalPosition => throw new System.NotImplementedException();

        public override void OnAdd()
        {
            SceneNode = new Godot.Spatial
            {
                Name = $"Transform {Owner.Uid} ({Owner.Name})"
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
            SceneNode.Rotation = new Godot.Vector3((float)rotationx - MathHelper.PiOver2, (float)rotationy, (float)rotationz);
        }

        public override void OnRemove()
        {
            base.OnRemove();

            SceneNode = null;
        }
    }
}
