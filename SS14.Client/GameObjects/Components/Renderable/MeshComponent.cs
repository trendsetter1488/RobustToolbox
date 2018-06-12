using SS14.Client.GameObjects.Components.Transform;
using SS14.Client.Graphics;
using SS14.Client.Graphics.ClientEye;
using SS14.Client.Interfaces.GameObjects.Components;
using SS14.Client.Interfaces.ResourceManagement;
using SS14.Client.Utility;
using SS14.Shared.GameObjects;
using SS14.Shared.IoC;
using SS14.Shared.Log;
using SS14.Shared.Maths;
using SS14.Shared.Prototypes;
using SS14.Shared.Utility;
using System;
using YamlDotNet.RepresentationModel;

namespace SS14.Client.GameObjects
{
    public sealed class MeshComponent : Component
    {
        public override string Name => "Mesh";
        public override uint? NetID => NetIDs.MESH;
        public override Type StateType => typeof(MeshComponentState);

        private Godot.PackedScene MasterScene;
        private Godot.MeshInstance SceneNode;
        private string _scenename;
        private IGodotTransformComponent TransformComponent;

        private IResourceCache resourceCache;
        private IPrototypeManager prototypes;

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                SceneNode.Visible = value;
            }
        }

        private Vector3 scale = Vector3.One;
        /// <summary>
        ///     A scale applied to all layers.
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                if (SceneNode != null)
                {
                    SceneNode.Scale = value.Convert();
                }
            }
        }

        private Vector3 rotation;
        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (SceneNode != null)
                {
                    SceneNode.Rotation = value.Convert();
                }
            }
        }

        private Vector3 offset = Vector3.Zero;
        /// <summary>
        ///     Offset applied to all layers.
        /// </summary>
        public Vector3 Offset
        {
            get => offset;
            set
            {
                offset = value;
                if (SceneNode != null)
                {
                    SceneNode.Translation = value.Convert();
                }
            }
        }

        //private Color color = Color.White;
        //public Color Color
        //{
        //    get => color;
        //    set
        //    {
        //        color = value;
        //        if (SceneNode != null)
        //        {
        //            SceneNode.material
        //            SceneNode.GetSurfaceMaterial().Color = value.Convert();
        //        }
        //    }
        //}
        
        public void SetSurfaceMaterial(int surface, Godot.Material material)
        {
            try
            {
                SceneNode.SetSurfaceMaterial(surface, material);
            }
            catch
            {
                Logger.Info(string.Format("Meshed failed to set new material on surface {0}! Trace:\n{1}", surface, Environment.StackTrace));
            }
        }

        //public void SetSurfaceShader(int surface, string shaderName)
        //{
        //    if (!prototypes.TryIndex<ShaderPrototype>(shaderName, out var prototype))
        //    {
        //        Logger.ErrorS("go.comp.mesh", "Shader prototype '{0}' does not exist. Trace:\n{1}", shaderName, Environment.StackTrace);
        //    }

        //    if(prototype.Kind == ShaderPrototype.ShaderKind.Canvas)
        //    {
        //        Logger.ErrorS("go.comp.mesh", "Shader prototype '{0}' is a canvas item, cannot be applied to a mesh. Trace:\n{1}", shaderName, Environment.StackTrace);
        //        return;
        //    }

        //    try
        //    {
        //        var material = (Godot.SpatialMaterial)SceneNode.GetSurfaceMaterial(surface);
        //        material.Shader = ShaderPrototype.Shader;
        //        SceneNode.SetSurfaceMaterial(surface, material);
                
        //    }
        //    catch
        //    {
        //        Logger.ErrorS("go.comp.mesh", "Meshed failed to set new material on surface {0}! Trace:\n{1}", surface, Environment.StackTrace);
        //    }
        //}

        public void SetSurfaceTexture(int surface, Texture texture)
        {
            try
            {
                var material = (Godot.SpatialMaterial)SceneNode.GetSurfaceMaterial(surface);
                material.AlbedoTexture = texture;
                SceneNode.SetSurfaceMaterial(surface, material);
            }
            catch
            {
                Logger.Info(string.Format("Meshed failed to set new material on surface {0}! Trace:\n{1}", surface, Environment.StackTrace));
            }
        }

        public void SetMeshInstance(string meshinstancename)
        {
            if(string.IsNullOrEmpty(meshinstancename))
            {
                return;
            }

            Logger.Info(string.Format("setmeshinstance to {0}", meshinstancename));

            if (MasterScene == null)
            {
                Logger.Info(string.Format("No packed scene to pull new state from! Trace:\n{0}", Environment.StackTrace));
            }
            else
            {
                var scene = MasterScene.Instance();
                var newmeshinstance = scene.GetNode(meshinstancename);
                Logger.Info(string.Format("type is {0}", newmeshinstance.GetType().ToString()));

                if(Owner.TryGetComponent(out Transform3DGodot transform))
                {
                    TransformComponent = transform;
                }


                if(newmeshinstance != null)
                {
                    Logger.Info(string.Format("success on mesh {0}", meshinstancename));
                    if (SceneNode != null)
                    {
                        if(TransformComponent != null)
                        {
                            TransformComponent.Node.RemoveChild(SceneNode);
                        }
                        SceneNode.QueueFree();
                    }

                    SceneNode = (Godot.MeshInstance)newmeshinstance;
                    SceneNode.GetParent().RemoveChild(SceneNode);
                    SceneNode.Visible = _visible;
                    SceneNode.Rotation = rotation.Convert();
                    SceneNode.Translation = offset.Convert();
                    SceneNode.Scale = scale.Convert();
                    if(TransformComponent != null)
                    {
                        Logger.Info(string.Format("added new mesh to scene {0}", meshinstancename));
                        TransformComponent.Node.AddChild(SceneNode);
                    }
                    
                    scene.QueueFree();
                }
                else
                {
                    Logger.Info(string.Format("Could not find meshinstance {0} in packed scene {1}! Trace:\n{2}", meshinstancename, MasterScene.ResourcePath,Environment.StackTrace));
                }
            }
        }

        public void SetPackedScene(string scenefilename)
        {
            if(_scenename != scenefilename)
            try
            {
                MasterScene = (Godot.PackedScene)Godot.GD.Load("res://models/content/" + scenefilename + ".dae");
                _scenename = scenefilename;
            }
            catch
            {
                Logger.Info(string.Format("Hey server, the packed scene '{0}' doesn't exist.", scenefilename));
            }
        }

        public void SetSceneAndMeshInstance(string scenefilename, string meshinstance)
        {
            SetPackedScene(scenefilename);
            SetMeshInstance(meshinstance);
        }

        public override void OnAdd()
        {
            base.OnAdd();
            //SceneNode = new Godot.MeshInstance()
            //{
            //    Mesh = new Godot.CubeMesh(),
            //    Scale = new Godot.Vector3(0.5f, 0.5f, 0.5f),
            //    Translation = offset.Convert(),
            //    Rotation = new Godot.Vector3(0, 0, 0)
            //};
        }

        public override void OnRemove()
        {
            base.OnRemove();
            
            SceneNode.QueueFree();
        }

        public override void Initialize()
        {
            base.Initialize();

            if(Owner.TryGetComponent(out Transform3DGodot transform))
            {
                TransformComponent = transform;
                TransformComponent.Node.AddChild(SceneNode);
            }
        }

        public override void LoadParameters(YamlMappingNode mapping)
        {
            base.LoadParameters(mapping);

            prototypes = IoCManager.Resolve<IPrototypeManager>();
            resourceCache = IoCManager.Resolve<IResourceCache>();

            if (mapping.TryGetNode("scene", out var node))
            {
                try
                {
                    MasterScene = (Godot.PackedScene)Godot.GD.Load("res://models/content/" + node.AsString() + ".dae");
                }
                catch
                {
                    Logger.Info(string.Format("Unable to load RSI '{0}'. Prototype: '{1}'", node.AsString(), Owner.Prototype.ID));
                }
            }

            if (mapping.TryGetNode("state", out node))
            {
                if (MasterScene == null)
                {
                    Logger.Info(string.Format("No base packed scene set to load the mesh instance from: "
                                  + "cannot use 'state' property. Prototype: '{0}'", Owner.Prototype.ID));
                }
                else
                {
                    SetMeshInstance(node.AsString());
                }
            }

            if (mapping.TryGetNode("scale", out node))
            {
                Scale = node.AsVector3();
            }

            if (mapping.TryGetNode("rotation", out node))
            {
                Rotation = node.AsVector3();
            }

            if (mapping.TryGetNode("offset", out node))
            {
                Offset = node.AsVector3();
            }

            if (mapping.TryGetNode("visible", out node))
            {
                Visible = node.AsBool();
            }
        }

        public override void HandleComponentState(ComponentState state)
        {
            var thestate = (MeshComponentState)state;

            Visible = thestate.Visible;
            Scale = thestate.Scale;
            Rotation = thestate.Rotation;
            Offset = thestate.Offset;

            if (thestate.ScenePath != null && MasterScene != null)
            {
                SetPackedScene(thestate.ScenePath);
                SetMeshInstance(thestate.MeshInstance);
            }
        }
    }
}
