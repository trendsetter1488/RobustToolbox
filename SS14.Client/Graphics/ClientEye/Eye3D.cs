using System;
using SS14.Client.Interfaces.Graphics.ClientEye;
using SS14.Client.Utility;
using SS14.Shared.IoC;
using SS14.Shared.Log;
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
                    GodotCamera.Current = true;
                    if (eyeManager.CurrentEye != this)
                    {
                        eyeManager.CurrentEye = this;
                    }
                }
                else
                {
                    GodotCamera.Current = false;
                    if (eyeManager.CurrentEye == this)
                    {
                        eyeManager.CurrentEye = null;
                    }
                }
            }
        }

        public MapId MapId { get; set; } = MapId.Nullspace;

        public Eye3D()
        {
            var environment = new Godot.Environment()
            {
                BackgroundMode = Godot.Environment.BGMode.Sky,
                BackgroundSky = new Godot.ProceduralSky(),
            };

            GodotCamera = new Godot.Camera()
            {
                Environment = environment,
                Fov = 90,
                Projection = Godot.Camera.ProjectionEnum.Perspective,
                RotationDegrees = new Godot.Vector3(0, 0, 0),
                Translation = new Godot.Vector3(0, 0, 10),
                Scale = new Godot.Vector3(1, 1, 1),
                //Size = 10
            };
            eyeManager = IoCManager.Resolve<IEyeManager>();
        }

        public Vector2 WorldToScreen(Vector2 point)
        {
            return GodotCamera.UnprojectPosition(new Godot.Vector3(point.X, point.Y, 0f)).Convert();
        }

        private Random random = new Random();

        /// <summary>
        /// This function returns the intersection of a screen point with the scalar plane plane specified by your argument.
        /// Defaults to intersection with z = 0. To get intersection with any z value you can pass Vector4(0, 0, 1, z)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scalarplaneequation"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 point, Vector4 scalarplaneequation = new Vector4())
        {
            if(scalarplaneequation == new Vector4(0, 0, 0, 0))
            {
                scalarplaneequation = new Vector4(0, 0, 1, 0);
            }
            var cameraorigin = GodotCamera.GetGlobalTransform().origin;
            var directionray = GodotCamera.ProjectRayNormal(point.Convert());
            var normal = new Godot.Vector3(scalarplaneequation.X, scalarplaneequation.Y, scalarplaneequation.Z);
            var denominator = directionray.Dot(normal);

            if(denominator == 0)
            {
                return new Vector2(0, 0); //now you have fucked up
            }

            var numerator = scalarplaneequation.W - normal.Dot(cameraorigin);
            var distancefromcameraorigin = numerator / denominator;
            var planeintersectionpoint = distancefromcameraorigin * directionray + cameraorigin;

            //if(random.Next() > int.MaxValue/30)
            //    Logger.Info(string.Format("Oh fuck distance {0}, directionraynormal {1}, numerator {2}, planeintersection {3} {4} {5}",
            //        distancefromcameraorigin,
            //        GodotCamera.ProjectRayNormal(point.Convert()),
            //        numerator,
            //        planeintersectionpoint.x,
            //        planeintersectionpoint.y,
            //        planeintersectionpoint.z));
            
            return new Vector2(planeintersectionpoint.x, planeintersectionpoint.y);
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
