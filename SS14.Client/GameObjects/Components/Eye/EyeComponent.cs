using SS14.Client.Graphics.ClientEye;
using SS14.Client.Interfaces.GameObjects.Components;
using SS14.Client.Interfaces.Graphics.ClientEye;
using SS14.Shared.GameObjects;

namespace SS14.Client.GameObjects
{
    public class EyeComponent : Component
    {
        public override string Name => "Eye";

        private IEye eye;

        // Horrible hack to get around ordering issues.
        private bool setCurrentOnInitialize = false;
        public bool Current
        {
            get => eye.Current;
            set
            {
                if (eye == null)
                {
                    setCurrentOnInitialize = value;
                    return;
                }
                eye.Current = value;
            }
        }

        IGodotTransformComponent transform;

        public override void Initialize()
        {
            base.Initialize();
            transform = Owner.GetComponent<IGodotTransformComponent>();

            eye = EyeManager.NewDefaultEye(setCurrentOnInitialize);

            transform.Node.AddChild(eye.Camera);
            transform.OnMove += Transform_OnMove;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            transform.OnMove -= Transform_OnMove;
            //TODO: Dispose
            //eye.Dispose();
        }

        private void Transform_OnMove(object sender, Shared.Enums.MoveEventArgs e)
        {
            eye.MapId = e.NewPosition.MapID;
        }
    }
}
