using SS14.Client.Interfaces.GameObjects.Components;
using SS14.Client.Interfaces.Graphics.Lighting;
using SS14.Client.Utility;
using SS14.Shared;
using SS14.Shared.Maths;
using System;
using SS14.Shared.Enums;

namespace SS14.Client.Graphics.Lighting
{
    partial class LightManager
    {
        sealed class Light : ILight
        {
            public Vector2 Offset
            {
                get => new Vector2(Light2D.GlobalTransform.origin.x, Light2D.GlobalTransform.origin.y);
                set { }
            }

            public Angle Rotation
            {
                get => new Angle(0);
                set { }
            }

            private Color color;
            public Color Color
            {
                get => color;
                set
                {
                    if (value == color)
                    {
                        return;
                    }

                    color = value;
                    //Light2D.Color = value.Convert();
                }
            }

            private float textureScale;
            public float TextureScale
            {
                get => textureScale;
                set
                {
                    if (value == textureScale)
                    {
                        return;
                    }

                    textureScale = value;
                    //Light2D.TextureScale = value;
                }
            }

            private float energy;
            public float Energy
            {
                get => energy;
                set
                {
                    if (value == energy)
                    {
                        return;
                    }

                    energy = value;
                    //Light2D.Energy = value;
                }
            }

            public ILightMode Mode { get; private set; }

            public LightModeClass ModeClass
            {
                get => Mode.ModeClass;
                set
                {
                    if (value == ModeClass)
                    {
                        return;
                    }

                    // In this order so IF something blows up making the new instance,
                    // this light doesn't corrupt completely and throw exceptions everywhere.
                    var newMode = GetModeInstance(value);
                    Mode.Shutdown();
                    Mode = newMode;
                    Mode.Start(this);
                }
            }

            private Texture texture;
            public Texture Texture
            {
                get => texture;
                set
                {
                    if (texture == value)
                    {
                        return;
                    }
                    texture = value;
                    //Light2D.Texture = value;
                }
            }

            private bool enabled;
            public bool Enabled
            {
                get => enabled;
                set
                {
                    enabled = value;
                    UpdateEnabled();
                }
            }

            private Godot.OmniLight Light2D;
            private LightManager Manager;
            private bool Deferred => Manager.Deferred;

            private IGodotTransformComponent parentTransform;
            private Godot.Vector2 CurrentPos;

            public Light(LightManager manager)
            {
                //Light2D = new Godot.Light2D()
                //{
                //    // TODO: Allow this to be modified.
                //    ShadowEnabled = true,
                //    ShadowFilter = Godot.Light2D.ShadowFilterEnum.Pcf13,
                //    ShadowGradientLength = 1,
                //    ShadowFilterSmooth = 0.25f,
                //};

                Light2D = new Godot.OmniLight()
                {
                    OmniRange = 10,
                    OmniAttenuation = 1,
                    Translation = new Godot.Vector3(0, 0, 0),
                };

                Manager = manager;
                Mode = new LightModeConstant();
                Mode.Start(this);

                if (Deferred)
                {
                    Manager.deferredViewport.AddChild(Light2D);
                }
            }

            public void DeParent()
            {
                if (Deferred)
                {
                    //Light2D.Position = new Godot.Vector2(0, 0);
                }
                else
                {
                    parentTransform.Node.RemoveChild(Light2D);
                }
                UpdateEnabled();
            }

            public void ParentTo(IGodotTransformComponent node)
            {
                if (!Deferred)
                {
                    node.Node.AddChild(Light2D);
                }
                parentTransform = node;
                UpdateEnabled();
            }

            public void Dispose()
            {
                // Already disposed.
                if (Light2D == null)
                {
                    return;
                }

                Manager.RemoveLight(this);
                Manager = null;

                Light2D.QueueFree();
                Light2D.Dispose();
                Light2D = null;
            }

            private static ILightMode GetModeInstance(LightModeClass modeClass)
            {
                switch (modeClass)
                {
                    case LightModeClass.Constant:
                        return new LightModeConstant();

                    default:
                        throw new NotImplementedException("Light modes outside Constant are not implemented yet.");
                }
            }

            public void UpdateEnabled()
            {
                Light2D.Visible = Enabled && Manager.Enabled && parentTransform != null;
            }

            public void FrameProcess(FrameEventArgs args)
            {
                // TODO: Maybe use OnMove events to make this less expensive.
                if (Deferred && parentTransform != null)
                {
                    var newpos = parentTransform.GlobalPosition;
                    if (CurrentPos != newpos)
                    {
                        //Light2D.Position = newpos;
                    }
                }
            }
        }
    }
}
