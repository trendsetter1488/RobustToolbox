using System;
using SS14.Client.Interfaces.Graphics.ClientEye;
using SS14.Client.Utility;
using SS14.Shared.Interfaces.Map;
using SS14.Shared.IoC;
using SS14.Shared.Map;
using SS14.Shared.Maths;

namespace SS14.Client.Graphics.ClientEye
{
    public class Eye2D : IEye, IDisposable
    {
        protected IEyeManager eyeManager;
        public Godot.Node Camera => GodotCamera;

        public Godot.Camera2D GodotCamera { get; private set; }

        private bool disposed = false;

        public bool Current
        {
            get => eyeManager.CurrentEye == this;
            set
            {
                if (Current == value)
                {
                    return;
                }

                if (value)
                {
                    GodotCamera.Current = true;
                    if(eyeManager.CurrentEye != this)
                    {
                        eyeManager.CurrentEye = this;
                    }
                }
                else
                {
                    GodotCamera.Current = false;
                    if(eyeManager.CurrentEye == this)
                    {
                        eyeManager.CurrentEye = null;
                    }
                }
            }
        }

        public MapId MapId { get; set; } = MapId.Nullspace;

        public Eye2D()
        {
            GodotCamera = new Godot.Camera2D()
            {
                DragMarginHEnabled = false,
                DragMarginVEnabled = false,
            };
            eyeManager = IoCManager.Resolve<IEyeManager>();
        }

        public Vector2 WorldToScreen(Vector2 point, Vector3 intersectionplane3d = new Vector3())
        {
            Godot.Node2D worldroot2d = (Godot.Node2D)eyeManager.sceneTree.WorldRoot;
            var transform = worldroot2d.GetViewportTransform();
            return transform.Xform(point.Convert() * EyeManager.PIXELSPERMETER).Convert();
        }

        public Vector2 ScreenToWorld(Vector2 point)
        {
            Godot.Node2D worldroot2d = (Godot.Node2D)eyeManager.sceneTree.WorldRoot;
            var transform = worldroot2d.GetViewportTransform();
            return transform.XformInv(point.Convert()).Convert() / EyeManager.PIXELSPERMETER;
        }

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;

            if (disposing)
            {
                Current = false;
                eyeManager = null;
            }

            GodotCamera.QueueFree();
            GodotCamera.Dispose();
            GodotCamera = null;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Eye2D()
        {
            Dispose(false);
        }
    }
}
