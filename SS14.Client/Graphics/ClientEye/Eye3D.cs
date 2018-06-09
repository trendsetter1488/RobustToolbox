using System;
using SS14.Client.Interfaces.Graphics.ClientEye;
using SS14.Client.Utility;
using SS14.Shared.IoC;
using SS14.Shared.Map;
using SS14.Shared.Maths;

namespace SS14.Client.Graphics.ClientEye
{
    public class Eye3D : IEye, IDisposable
    {
        protected IEyeManager eyeManager;

        public Godot.Node Camera => GodotCamera;

        public Godot.Camera GodotCamera { get; private set; }

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
                    eyeManager.CurrentEye = this;
                    GodotCamera.Current = true;
                }
                else
                {
                    eyeManager.CurrentEye = null;
                    GodotCamera.Current = false;
                }
            }
        }

        public MapId MapId { get; set; } = MapId.Nullspace;

        public Eye3D()
        {
            GodotCamera = new Godot.Camera();
            eyeManager = IoCManager.Resolve<IEyeManager>();
        }

        public Vector2 WorldToScreen(Vector2 point, Vector3 intersectionplane3d = new Vector3())
        {
            //return GodotCamera.UnprojectPosition(new Godot.Vector3(point.X, point.Y, 0f)).Convert();
            return new Vector2(0, 0);
        }
        
        public Vector2 ScreenToWorld(Vector2 point)
        {
            //var directionray = GodotCamera.ProjectRayNormal(point.Convert());
            //Godot.Node2D worldroot2d = (Godot.Node2D)eyeManager.sceneTree.WorldRoot;
            //var transform = worldroot2d.GetViewportTransform();
            //return transform.XformInv(point.Convert()).Convert() / EyeManager.PIXELSPERMETER;
            return new Vector2(0, 0);
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

        ~Eye3D()
        {
            Dispose(false);
        }
    }
}
