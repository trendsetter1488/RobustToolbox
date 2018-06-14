using SS14.Client.Graphics.ClientEye;
using SS14.Client.Utility;
using SS14.Shared.Maths;

namespace SS14.Client.GameObjects.Components.Transform
{
    public class Transform3DGodot : GodotTransformComponent
    {
        public Godot.Spatial SceneNode { get; private set; }

        public override Godot.Node Node => SceneNode;

        public override Godot.Vector2 GlobalPosition => new Godot.Vector2(SceneNode.Translation.x, SceneNode.Translation.y);

        public override void OnAdd()
        {
            SceneNode = new Godot.Spatial
            {
                Name = $"Transform {Owner.Uid} ({Owner.Name})",
                Translation = new Godot.Vector3(0, 0, 1)
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
            SceneNode.Rotation = new Godot.Vector3((float)rotationx, (float)rotationy, (float)rotationz);
        }

        protected override void SetPosition(float positionx, float positiony, float positionz = 1)
        {
            base.SetPosition(positionx, positiony, positionz);
            var position = new Vector3(positionx, positiony, positionz);
            SceneNode.Translation = (position).Convert();
        }

        public override void OnRemove()
        {
            base.OnRemove();

            SceneNode = null;
        }
    }
}
